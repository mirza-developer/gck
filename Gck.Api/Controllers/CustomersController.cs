using Gck.Application.DTOs;
using Gck.Application.Features.Customers.Commands.CreateCustomer;
using Gck.Application.Features.Customers.Commands.DeleteCustomer;
using Gck.Application.Features.Customers.Commands.IntroduceCustomer;
using Gck.Application.Features.Customers.Commands.UpdateCustomer;
using Gck.Application.Features.Customers.Commands.VerifyReferral;
using Gck.Application.Features.Customers.Queries.GetAllCustomers;
using Gck.Application.Features.Customers.Queries.GetCustomerById;
using Gck.Application.Features.Customers.Queries.GetPendingReferrals;
using Gck.Persistence;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gck.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CustomersController> _logger;
    private readonly GckDbContext _context;

    public CustomersController(IMediator mediator, ILogger<CustomersController> logger, GckDbContext context)
    {
        _mediator = mediator;
        _logger = logger;
        _context = context;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<CustomerDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CustomerDto>>> GetAll()
    {
        var customers = await _mediator.Send(new GetAllCustomersQuery());
        return Ok(customers);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerDto>> GetById(int id)
    {
        var customer = await _mediator.Send(new GetCustomerByIdQuery { Id = id });
        if (customer == null)
            {
                return NotFound($"Customer with ID '{id}' not found");
            }
        return Ok(customer);
    }

    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<int>> Create([FromBody] CreateCustomerCommand command)
    {
        var customerId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = customerId }, customerId);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCustomerCommand command)
    {
        if (id != command.Id)
        {
        return BadRequest("ID in URL does not match ID in request body");
        }

        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _mediator.Send(new DeleteCustomerCommand { Id = id });
        return NoContent();
    }

    [HttpGet("{id}/sessions")]
    [ProducesResponseType(typeof(List<CustomerSessionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CustomerSessionDto>>> GetCustomerSessions(int id)
    {
        var sessions = await _context.SessionCustomers
                .Where(sc => sc.CustomerId == id)
                .Include(sc => sc.Session)
                    .ThenInclude(s => s.Table)
                .OrderByDescending(sc => sc.Session.StartDateTime)
                .Select(sc => new CustomerSessionDto
                {
                    Id = sc.Session.Id,
                    StartDateTime = sc.Session.StartDateTime,
                    EndDateTime = sc.Session.EndDateTime,
                    TableName = sc.Session.Table.Name,
                    IsFreeSession = sc.Session.IsFreeSession,
                    FinalPrice = sc.Session.FinalPrice
                })
                .ToListAsync();

        return Ok(sessions);
    }

    [HttpGet("referrals/pending")]
    [ProducesResponseType(typeof(List<CustomerDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CustomerDto>>> GetPendingReferrals()
    {
        var pending = await _mediator.Send(new GetPendingReferralsQuery());
        return Ok(pending);
    }

    [HttpPost("{id}/verify-referral")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> VerifyReferral(int id, [FromBody] VerifyReferralCommand command)
    {
        command.CustomerId = id;
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpPost("introduce")]
    [ProducesResponseType(typeof(IntroduceCustomerResult), StatusCodes.Status200OK)]
    public async Task<ActionResult<IntroduceCustomerResult>> IntroduceCustomer([FromBody] IntroduceCustomerCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}

public class CustomerSessionDto
{
    public int Id { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime? EndDateTime { get; set; }
    public string TableName { get; set; } = string.Empty;
    public bool IsFreeSession { get; set; }
    public decimal? FinalPrice { get; set; }
}
