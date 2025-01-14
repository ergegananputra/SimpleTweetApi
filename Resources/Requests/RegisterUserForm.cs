namespace SimpleTweetApi.Resources.Requests;

public record RegisterUserForm(
    string Username,
    string Email,
    string Password
    );
