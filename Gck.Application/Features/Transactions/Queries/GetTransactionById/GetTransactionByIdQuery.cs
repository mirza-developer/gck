using Gck.Application.DTOs;
using MediatR;

namespace Gck.Application.Features.Transactions.Queries.GetTransactionById;

public class GetTransactionByIdQuery : IRequest<TransactionDto?>
{
    public int Id { get; set; }
}
