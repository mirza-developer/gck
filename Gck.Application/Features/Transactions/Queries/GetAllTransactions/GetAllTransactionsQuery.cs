using Gck.Application.DTOs;
using MediatR;

namespace Gck.Application.Features.Transactions.Queries.GetAllTransactions;

public class GetAllTransactionsQuery : IRequest<List<TransactionDto>>
{
}
