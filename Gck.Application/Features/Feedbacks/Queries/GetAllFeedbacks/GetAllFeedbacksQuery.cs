using Gck.Application.DTOs;
using MediatR;

namespace Gck.Application.Features.Feedbacks.Queries.GetAllFeedbacks;

public class GetAllFeedbacksQuery : IRequest<List<FeedbackDto>>
{
}
