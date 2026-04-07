namespace EventDrivenBookingPlatform.Modules.Notifications.Infrastructure.Persistence;

public class NotificationMessage
{
    public Guid Id { get; set; }
    public string Recipient { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string Channel { get; set; } = "Email";
    public string Status { get; set; } = "Sent";
    public DateTime SentAt { get; set; }
}
