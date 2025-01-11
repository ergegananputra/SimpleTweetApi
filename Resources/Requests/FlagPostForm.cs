namespace SimpleTweetApi.Resources.Requests;

public record FlagPostForm(
    string? Name,
    string? Description,
    string? Icon = null
);
