using Gck.Application.Common.Helpers;
using Gck.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gck.Application.Features.Users.Commands.UpdateUserPassword;

public class UpdateUserPasswordCommandHandler : IRequestHandler<UpdateUserPasswordCommand, Unit>
{
    private readonly GckDbContext _context;

    public UpdateUserPasswordCommandHandler(GckDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateUserPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID '{request.UserId}' not found.");
        }

        // Verify current password
        if (!PasswordHasher.VerifyPassword(request.CurrentPassword, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Current password is incorrect.");
        }

        // Update to new password
        user.PasswordHash = PasswordHasher.HashPassword(request.NewPassword);
        user.LastModifiedDate = DateTime.Now;
        user.LastModifierIdentityID = "system"; // You can change this to actual user ID

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
