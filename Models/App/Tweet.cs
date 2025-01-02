using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace SimpleTweetApi.Models.App;

public class Tweet : BaseModel
{
    [Key]
    public Guid Uuid { get; set; } = Guid.NewGuid();
    public string Content { get; set; }
    public long Likes { get; set; }
    public string? Flags { get; set; } = null;
    public string UserId { get; set; }
}
