using EventDrivenBookingPlatform.Modules.Notifications.Application.Abstractions;
using EventDrivenBookingPlatform.Modules.Notifications.Infrastructure.Persistence;
using Microsoft.Extensions.Logging;

namespace EventDrivenBookingPlatform.Modules.Notifications.Infrastructure;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly NotificationsDbContext _context;

    public EmailService(ILogger<EmailService> logger, NotificationsDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        await _context.NotificationMessages.AddAsync(new NotificationMessage
        {
            Id = Guid.NewGuid(),
            Recipient = to,
            Subject = subject,
            Body = body,
            Channel = "Email",
            Status = "Sent",
            SentAt = DateTime.UtcNow
        }, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Fake email sent to {To}. Subject: {Subject}. Body: {Body}", to, subject, body);
    }
}
