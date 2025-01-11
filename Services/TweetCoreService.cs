using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SimpleTweetApi.Database;
using SimpleTweetApi.Models.App;

namespace SimpleTweetApi.Services;

public class TweetCoreService
{
    private readonly ApplicationDbContext _context;

    public TweetCoreService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Tweet>> Tweets(string? keyword = null, int page = 1, int limit = 20)
    {
        var query = _context.Tweets
            .Where(t => t.DeletedAt == null)
            .Where(t => !t.Flags.Contains("REPORT"))
            .OrderByDescending(t => t.CreatedAt)
            .AsQueryable();

        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(t => t.Content.Contains(keyword));
        }

        return await query
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<Tweet?> Tweet(Guid uuid)
    {
        var tweet = await _context.Tweets.FindAsync(uuid);

        if (tweet == null || tweet.DeletedAt != null)
        {
            return null;
        }

        return tweet;
    }

    public async Task<Tweet> Create(Tweet tweet)
    {
        _context.Tweets.Add(tweet);
        await _context.SaveChangesAsync();
        return tweet;
    }

    public async Task<Tweet?> Update(Guid uuid, Tweet tweet)
    {
        var existingTweet = await _context.Tweets.FindAsync(uuid);
        if (existingTweet == null || existingTweet.DeletedAt != null)
        {
            return null;
        }
        existingTweet.Content = tweet.Content;
        existingTweet.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return existingTweet;
    }

    public async Task<bool> Delete(Guid uuid)
    {
        var tweet = await _context.Tweets.FindAsync(uuid);
        if (tweet == null || tweet.DeletedAt != null)
        {
            return false;
        }
        tweet.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> Like(Guid uuid, string userId)
    {
        //TODO: Temporary Implementation, fix this later
        var tweet = await _context.Tweets.FindAsync(uuid);
        if (tweet == null || tweet.DeletedAt != null)
        {
            return false;
        }

        var tweetLikes = await _context.TweetLikes.FindAsync(tweet.Uuid, tweet.UserId);

        if (tweetLikes != null)
        {
            return false;
        }

        tweetLikes = new TweetLikes
        {
            TweetUuid = tweet.Uuid,
            UserId = userId,
        };

        try
        {
            await _context.Database.BeginTransactionAsync();
            await _context.TweetLikes.AddAsync(tweetLikes);

            int newTweetLikeCount = await _context.TweetLikes
                .Where(tl => tl.TweetUuid == tweet.Uuid)
                .Where(tl => tl.DeletedAt == null)
                .CountAsync();

            tweet.Likes = newTweetLikeCount;

            await _context.SaveChangesAsync();

            await _context.Database.CommitTransactionAsync();
            return true;
        }
        catch (DbUpdateException)
        {
            await _context.Database.RollbackTransactionAsync();
            return false;
        }

        
    }

    public async Task<bool> Unlike(Guid uuid, string userId)
    {
        //TODO: Temporary Implementation, fix this later
        var tweet = await _context.Tweets.FindAsync(uuid);
        if (tweet == null || tweet.DeletedAt != null)
        {
            return false;
        }

        TweetLikes? tweetLikes = await _context.TweetLikes.FindAsync(tweet.Uuid, userId);

        if (tweetLikes == null)
        {
            return false;
        }

        try
        {
            await _context.Database.BeginTransactionAsync();
            await _context.TweetLikes.AddAsync(tweetLikes);

            int newTweetLikeCount = await _context.TweetLikes
                .Where(tl => tl.TweetUuid == tweet.Uuid)
                .Where(tl => tl.DeletedAt == null)
                .CountAsync();

            tweet.Likes = newTweetLikeCount;

            await _context.SaveChangesAsync();

            await _context.Database.CommitTransactionAsync();
            return true;
        }
        catch (DbUpdateException)
        {
            await _context.Database.RollbackTransactionAsync();
            return false;
        }
    }

    public async Task<bool> Flag(Guid uuid, string flagCode, string userId, string note)
    {
        var tweet = await _context.Tweets.FindAsync(uuid);
        if (tweet == null || tweet.DeletedAt != null)
        {
            return false;
        }

        TweetFlags? tweetFlags = await _context.TweetFlags.FindAsync(tweet.Uuid, userId);

        if (tweetFlags != null)
        {
            return false;
        }

        tweetFlags = new TweetFlags
        {
            TweetUuid = tweet.Uuid,
            FlagCode = flagCode,
            ReporterUuid = userId,
            Note = note
        };

        try
        {
            await _context.Database.BeginTransactionAsync();
            await _context.TweetFlags.AddAsync(tweetFlags);

         
            if (flagCode.Contains("REPORT"))
            {
                // Count the number of flags for this tweet with the same flag code
                int newTweetFlagCount = await _context.TweetFlags
                    .Where(tf => tf.TweetUuid == tweet.Uuid)
                    .Where(tf => tf.FlagCode == flagCode)
                    .Where(tf => tf.DeletedAt == null)
                    .CountAsync();

                if (newTweetFlagCount >= 5)
                {
                    tweet.Flags = flagCode.Replace("PENDING_", "");
                }
            }

            await _context.SaveChangesAsync();
            await _context.Database.CommitTransactionAsync();
            return true;
        }
        catch (DbUpdateException)
        {
            await _context.Database.RollbackTransactionAsync();
            return false;
        }
    }


}
