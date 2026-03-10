using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Gck.Application.Features.Auth.Commands.Login;

public class LoginCommand : IRequest<LoginResponse>
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public string? Username { get; set; }
    public string? Name { get; set; }
}
