using Gck.Application.Common.Helpers;
using Gck.Domain.Entities;
using Gck.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gck.Application.Features.Users.Commands.AddUser;

public class AddUserCommandHandler : IRequestHandler<AddUserCommand, string>
{
    private readonly GckDbContext _context;

    public AddUserCommandHandler(GckDbContext context)
    {
        _context = context;
    }

    public async Task<string> Handle(AddUserCommand request, CancellationToken cancellationToken)
    {
        // Check if username already exists
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username, cancellationToken);

        if (existingUser != null)
        {
            throw new InvalidOperationException($"Username '{request.Username}' already exists.");
        }

        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Username = request.Username,
            Name = request.Name,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            Details = request.Details,
            PasswordHash = PasswordHasher.HashPassword(request.Password),
            IsActive = true,
            CreateDate = DateTime.UtcNow,
            LastModifiedDate = DateTime.UtcNow,
            CreatorIdentityID = "system" // You can change this to actual user ID
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}
