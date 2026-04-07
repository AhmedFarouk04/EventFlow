namespace EventDrivenBookingPlatform.Modules.Reservations.Domain.Rules;

public interface IBusinessRule
{
    bool IsBroken();
    string Message { get; }
}
