using Gck.Application.DTOs;
using MediatR;

namespace Gck.Application.Features.Sessions.Queries.GetSessionById;

public class GetSessionByIdQuery : IRequest<SessionDto?>
{
    public int Id { get; set; }
}
