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

    internal async Task<bool> Like(Guid uuid)
    {
        //TODO: Temporary Implementation, fix this later
        var tweet = await _context.Tweets.FindAsync(uuid);
        if (tweet == null || tweet.DeletedAt != null)
        {
            return false;
        }
        tweet.Likes++;
        await _context.SaveChangesAsync();
        return true;
    }

    internal async Task<bool> Unlike(Guid uuid)
    {
        //TODO: Temporary Implementation, fix this later
        var tweet = await _context.Tweets.FindAsync(uuid);
        if (tweet == null || tweet.DeletedAt != null)
        {
            return false;
        }
        tweet.Likes--;
        await _context.SaveChangesAsync();
        return true;
    }

    internal async Task<bool> Flag(Guid uuid, string flag)
    {
        var tweet = await _context.Tweets.FindAsync(uuid);
        if (tweet == null || tweet.DeletedAt != null)
        {
            return false;
        }

        tweet.Flags = flag;
        await _context.SaveChangesAsync();
        return true;
    }


}
