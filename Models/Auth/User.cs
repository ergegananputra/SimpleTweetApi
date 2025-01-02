using Microsoft.AspNetCore.Identity;

namespace SimpleTweetApi.Models.Auth;

public class User : IdentityUser
{
    public string? Bio { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? DeletedAt { get; set; }
}
