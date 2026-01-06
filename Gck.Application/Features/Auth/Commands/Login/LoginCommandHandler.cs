using Gck.Application.Common.Helpers;
using Gck.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gck.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly GckDbContext _context;

    public LoginCommandHandler(GckDbContext context)
    {
        _context = context;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username, cancellationToken);

        if (user == null)
        {
            return new LoginResponse
            {
                Success = false,
                Message = "نام کاربری یا رمز عبور اشتباه است"
            };
        }

        if (!user.IsActive)
        {
            return new LoginResponse
            {
                Success = false,
                Message = "حساب کاربری غیرفعال است"
            };
        }

        if (!PasswordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            return new LoginResponse
            {
                Success = false,
                Message = "نام کاربری یا رمز عبور اشتباه است"
            };
        }

        return new LoginResponse
        {
            Success = true,
            Message = "ورود موفقیت‌آمیز بود",
            UserId = user.Id,
            Username = user.Username,
            Name = user.Name
        };
    }
}
