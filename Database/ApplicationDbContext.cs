using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SimpleTweetApi.Models.App;
using SimpleTweetApi.Models.Auth;

namespace SimpleTweetApi.Database;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<User>(options)
{
    public DbSet<Tweet> Tweets { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Tweet>(entity =>
        {
            entity.Property(e => e.Uuid)
                .HasDefaultValueSql("gen_random_uuid()");

            entity.Property(e => e.Content)
                .IsRequired()
                .HasMaxLength(5000);

            entity.Property(e => e.Likes)
                .HasDefaultValue(0);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

        });
    }

}
