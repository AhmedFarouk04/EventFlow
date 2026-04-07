namespace EventDrivenBookingPlatform.Modules.Audit.Application.Models;

public class AuditLog
{
    public Guid Id { get; set; }
    public string EventName { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public DateTime LoggedAt { get; set; }
}
