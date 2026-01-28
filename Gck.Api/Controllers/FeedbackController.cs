using Gck.Application.DTOs;
using Gck.Application.Features.Feedbacks.Commands.MarkFeedbackAsRead;
using Gck.Application.Features.Feedbacks.Commands.SubmitFeedback;
using Gck.Application.Features.Feedbacks.Queries.GetAllFeedbacks;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Gck.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FeedbackController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<FeedbackController> _logger;

    public FeedbackController(IMediator mediator, ILogger<FeedbackController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> SubmitFeedback([FromBody] SubmitFeedbackDto dto)
    {
        try
        {
            var command = new SubmitFeedbackCommand
            {
                CustomerId = dto.CustomerId,
                Subject = dto.Subject,
                Message = dto.Message
            };

            var feedbackId = await _mediator.Send(command);

            return Ok(new { success = true, id = feedbackId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting feedback");
            return StatusCode(500, new { success = false, message = "خطا در ارسال بازخورد" });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllFeedbacks()
    {
        try
        {
            var query = new GetAllFeedbacksQuery();
            var feedbacks = await _mediator.Send(query);

            return Ok(feedbacks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading feedbacks");
            return StatusCode(500, new { message = "خطا در دریافت بازخوردها" });
        }
    }

    [HttpPut("{id}/mark-read")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        try
        {
            var command = new MarkFeedbackAsReadCommand { Id = id };
            var success = await _mediator.Send(command);

            if (!success)
                return NotFound();

            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking feedback as read");
            return StatusCode(500, new { success = false, message = "خطا در به‌روزرسانی وضعیت" });
        }
    }
}
