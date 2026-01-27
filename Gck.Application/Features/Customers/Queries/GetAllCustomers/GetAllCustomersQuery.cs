using Gck.Application.DTOs;
using MediatR;

namespace Gck.Application.Features.Customers.Queries.GetAllCustomers;

public class GetAllCustomersQuery : IRequest<List<CustomerDto>>
{
}
