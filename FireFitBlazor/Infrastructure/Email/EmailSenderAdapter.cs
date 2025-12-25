using FireFit.Shared.Email;
using NETCore.MailKit.Core;

namespace FireFitBlazor.Infrastructure.Email;

public class EmailSenderAdapter : IEmailSender
{
    private readonly IEmailService _mailKitEmailService;

    public EmailSenderAdapter(IEmailService mailKitEmailService)
    {
        _mailKitEmailService = mailKitEmailService;
    }

    public Task SendAsync(string to, string subject, string bodyHtml, CancellationToken ct = default)
        => _mailKitEmailService.SendAsync(to, subject, bodyHtml, true);
}
