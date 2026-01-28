using Gck.Application.DTOs;
using MediatR;

namespace Gck.Application.Features.Transactions.Queries.GetTransactionReport;

public class GetTransactionReportQuery : IRequest<TransactionReportDto>
{
    public int? FinancialAccountId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Type { get; set; }
}
