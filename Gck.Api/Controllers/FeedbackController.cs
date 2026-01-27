using Gck.Domain.Entities;
using Gck.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gck.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FeedbackController : ControllerBase
{
    private readonly GckDbContext _context;
    private readonly ILogger<FeedbackController> _logger;

    public FeedbackController(GckDbContext context, ILogger<FeedbackController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> SubmitFeedback([FromBody] SubmitFeedbackDto dto)
    {
        try
        {
            var feedback = new CustomerFeedback
            {
                CustomerId = dto.CustomerId,
                Subject = dto.Subject,
                Message = dto.Message,
                SubmittedAt = DateTime.UtcNow,
                IsRead = false
            };

            _context.CustomerFeedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            return Ok(new { success = true });
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
            var feedbacks = await _context.CustomerFeedbacks
                .Include(f => f.Customer)
                .OrderByDescending(f => f.SubmittedAt)
                .Select(f => new FeedbackDto
                {
                    Id = f.Id,
                    CustomerName = f.Customer.Name,
                    CustomerPhone = f.Customer.PhoneNumber,
                    Subject = f.Subject,
                    Message = f.Message,
                    SubmittedAt = f.SubmittedAt,
                    IsRead = f.IsRead
                })
                .ToListAsync();

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
            var feedback = await _context.CustomerFeedbacks.FindAsync(id);
            if (feedback == null)
                return NotFound();

            feedback.IsRead = true;
            await _context.SaveChangesAsync();

            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking feedback as read");
            return StatusCode(500, new { success = false, message = "خطا در به‌روزرسانی وضعیت" });
        }
    }
}

public class SubmitFeedbackDto
{
    public int CustomerId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public class FeedbackDto
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; }
    public bool IsRead { get; set; }
}
