namespace EventDrivenBookingPlatform.Modules.Reservations.Domain.Rules;

public interface IBusinessRule
{
    string Message { get; }
    bool IsBroken();
}