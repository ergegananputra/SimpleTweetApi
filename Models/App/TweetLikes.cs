using SimpleTweetApi.Models.Auth;

namespace SimpleTweetApi.Models.App;

public class TweetLikes : BaseModel
{
    public Guid TweetUuid { get; set; }
    public string UserId { get; set; }

    // Relations
    public Tweet Tweet { get; set; }
    public User User { get; set; }
}
