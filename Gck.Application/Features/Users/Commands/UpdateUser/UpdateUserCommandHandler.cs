using Gck.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gck.Application.Features.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Unit>
{
    private readonly GckDbContext _context;

    public UpdateUserCommandHandler(GckDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);

        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID '{request.Id}' not found.");
        }

        // Check if username is being changed and if it's already taken
        if (user.Username != request.Username)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username, cancellationToken);

            if (existingUser != null)
            {
                throw new InvalidOperationException($"Username '{request.Username}' already exists.");
            }
        }

        user.Username = request.Username;
        user.Name = request.Name;
        user.Email = request.Email;
        user.PhoneNumber = request.PhoneNumber;
        user.IsActive = request.IsActive;
        user.Details = request.Details;
        user.LastModifiedDate = DateTime.UtcNow;
        user.LastModifierIdentityID = "system"; // You can change this to actual user ID

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
