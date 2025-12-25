using Microsoft.AspNetCore.Mvc;
using NETCore.MailKit.Core;

namespace FireFitBlazor.Application.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailTestController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly IWebHostEnvironment _env;

        public EmailTestController(IEmailService emailService, IWebHostEnvironment env)
        {
            _emailService = emailService;
            _env = env;
        }

        public class TestEmailRequest
        {
            public string To { get; set; } = string.Empty;
        }

        [HttpPost("test")] // POST /api/emailtest/test { "to": "you@example.com" }
        public async Task<IActionResult> SendTestEmail([FromBody] TestEmailRequest req)
        {
            if (!_env.IsDevelopment())
            {
                return NotFound(); // disable in non-dev environments
            }

            if (string.IsNullOrWhiteSpace(req?.To))
            {
                return BadRequest("Missing 'to' email address");
            }

            try
            {
                var body = "<h3>FireFit test email</h3><p>This is a test.</p>";
                await _emailService.SendAsync(req.To, "FireFit Test Email", body, true);
                return Ok(new { sent = true, to = req.To });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { sent = false, error = ex.Message });
            }
        }
    }
}

