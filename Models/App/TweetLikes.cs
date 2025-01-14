using System.Text.Json.Serialization;
using SimpleTweetApi.Models.Auth;

namespace SimpleTweetApi.Models.App;

public class TweetLikes : BaseModel
{
    public Guid TweetUuid { get; set; }
    public string UserId { get; set; }

    // Relations
    [JsonIgnore]
    public Tweet Tweet { get; set; }
    [JsonIgnore]
    public User User { get; set; }
}
