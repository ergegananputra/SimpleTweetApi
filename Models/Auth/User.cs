using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using SimpleTweetApi.Models.App;

namespace SimpleTweetApi.Models.Auth;

public class User : IdentityUser
{
    public string? Bio { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? DeletedAt { get; set; }

    // Relations
    [JsonIgnore]
    public ICollection<Tweet> Tweets { get; set; } = new List<Tweet>();
    [JsonIgnore]
    public ICollection<TweetLikes> TweetsLiked { get; set; } = new List<TweetLikes>();
}
