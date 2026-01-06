using Gck.Application.DTOs;
using Gck.Application.Features.Customers.Commands.CreateCustomer;
using Gck.Application.Features.Customers.Queries.GetAllCustomers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Gck.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CustomersController> _logger;

    public CustomersController(IMediator mediator, ILogger<CustomersController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<CustomerDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CustomerDto>>> GetAll()
    {
        try
        {
            var customers = await _mediator.Send(new GetAllCustomersQuery());
            return Ok(customers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all customers");
            return StatusCode(500, "An error occurred while retrieving customers");
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<int>> Create([FromBody] CreateCustomerCommand command)
    {
        try
        {
            var customerId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetAll), new { id = customerId }, customerId);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating customer");
            return StatusCode(500, "An error occurred while creating the customer");
        }
    }
}
