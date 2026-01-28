using Gck.Application.DTOs;
using Gck.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gck.Application.Features.Feedbacks.Queries.GetAllFeedbacks;

public class GetAllFeedbacksQueryHandler : IRequestHandler<GetAllFeedbacksQuery, List<FeedbackDto>>
{
    private readonly GckDbContext _context;

    public GetAllFeedbacksQueryHandler(GckDbContext context)
    {
        _context = context;
    }

    public async Task<List<FeedbackDto>> Handle(GetAllFeedbacksQuery request, CancellationToken cancellationToken)
    {
        var feedbacks = await _context.CustomerFeedbacks
            .Include(f => f.Customer)
            .OrderByDescending(f => f.SubmittedAt)
            .Select(f => new FeedbackDto
            {
                Id = f.Id,
                CustomerName = f.Customer.Name,
                CustomerPhone = f.Customer.PhoneNumber,
                Subject = f.Subject,
                Message = f.Message,
                SubmittedAt = f.SubmittedAt,
                IsRead = f.IsRead
            })
            .ToListAsync(cancellationToken);

        return feedbacks;
    }
}
