using System.ComponentModel.DataAnnotations;
using System.Numerics;
using SimpleTweetApi.Models.Auth;

namespace SimpleTweetApi.Models.App;

public class Tweet : BaseModel
{
    [Key]
    public Guid Uuid { get; set; } = Guid.NewGuid();
    public string Content { get; set; }
    public long Likes { get; set; }
    public string? Flags { get; set; } = null;
    public string UserId { get; set; }

    // Relations
    public User User { get; set; }
    public ICollection<TweetLikes> UsersWhoLikes { get; set; } = new List<TweetLikes>();
}
