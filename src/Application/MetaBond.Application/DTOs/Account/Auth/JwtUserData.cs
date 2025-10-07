namespace MetaBond.Application.DTOs.Account.Auth;

public sealed class JwtUserData(string message, object? data = null)
{
    public string Message { get; set; } = message;
    public object? Data { get; set; } = data;
}