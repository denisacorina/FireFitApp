namespace FireFit.Shared.Email;

public interface IEmailSender
{
    Task SendAsync(string to, string subject, string bodyHtml, CancellationToken ct = default);
}

