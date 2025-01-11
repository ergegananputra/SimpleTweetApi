using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleTweetApi.Models;
using SimpleTweetApi.Models.App;
using SimpleTweetApi.Models.Auth;

namespace SimpleTweetApi.Database;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<User>(options)
{
    public DbSet<Tweet> Tweets { get; set; }
    public DbSet<TweetLikes> TweetLikes { get; set; }
    public DbSet<Flag> Flag { get; set; }
    public DbSet<TweetFlags> TweetFlags { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);


        BaseModelConfiguration(builder.Entity<Tweet>(), entity =>
        {
            entity.Property(e => e.Uuid)
                .HasDefaultValueSql("gen_random_uuid()");

            entity.Property(e => e.Content)
                .IsRequired()
                .HasMaxLength(5000);

            entity.Property(e => e.Likes)
                .HasDefaultValue(0);

            entity.HasOne(e => e.User)
                .WithMany(u => u.Tweets)
                .HasForeignKey(e => e.UserId);

            entity.HasMany(e => e.UsersWhoLikes)
                .WithOne(l => l.Tweet)
                .HasForeignKey(e => e.TweetUuid);
        });

        BaseModelConfiguration(builder.Entity<TweetLikes>(), entity =>
        {
            entity.HasKey(e => new { e.TweetUuid, e.UserId });

            entity.HasOne(e => e.Tweet)
                .WithMany(t => t.UsersWhoLikes)
                .HasForeignKey(e => e.TweetUuid);
            entity.HasOne<User>()
                .WithMany(u => u.TweetsLiked)
                .HasForeignKey(e => e.UserId);
        });

        BaseModelConfiguration(builder.Entity<Flag>(), entity =>
        {
            entity.HasKey(e => e.Code);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50);

        });

        BaseModelConfiguration(builder.Entity<TweetFlags>(), entity =>
        {
            entity.HasKey(e => new { e.TweetUuid, e.ReporterUuid });

            entity.Property(e => e.Note)
                .HasMaxLength(500);
        });
    }

    private static void BaseModelConfiguration<T>(EntityTypeBuilder<T> entity, Action<EntityTypeBuilder<T>>? buildAction = null) where T : BaseModel
    {
        entity.Property(e => e.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
        entity.Property(e => e.UpdatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        buildAction?.Invoke(entity);
    }

}
