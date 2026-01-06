using Gck.Application.DTOs;
using MediatR;

namespace Gck.Application.Features.Customers.Queries.GetCustomerById;

public class GetCustomerByIdQuery : IRequest<CustomerDto?>
{
    public int Id { get; set; }
}
