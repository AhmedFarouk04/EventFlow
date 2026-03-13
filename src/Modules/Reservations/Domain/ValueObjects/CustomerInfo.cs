using System.Text.RegularExpressions;
using EventDrivenBookingPlatform.BuildingBlocks.SharedKernel;

namespace EventDrivenBookingPlatform.Modules.Reservations.Domain.ValueObjects;

public class CustomerInfo : ValueObject
{
    public string FullName { get; }
    public string Email { get; }
    public string PhoneNumber { get; }
    public string Nationality { get; }
    public string? PassportNumber { get; }

    private CustomerInfo(string fullName, string email, string phoneNumber, string nationality, string? passportNumber)
    {
        FullName = fullName;
        Email = email.ToLowerInvariant();
        PhoneNumber = phoneNumber;
        Nationality = nationality.ToUpperInvariant();
        PassportNumber = passportNumber;
    }

    public static Result<CustomerInfo> Create(string fullName, string email, string phoneNumber, string nationality, string? passportNumber = null, bool isDomestic = false)
    {
        if (string.IsNullOrWhiteSpace(fullName) || fullName.Length < 2 || fullName.Length > 100)
            return Result<CustomerInfo>.Failure("Full name must be between 2 and 100 characters.");
        if (!Regex.IsMatch(fullName, @"^[a-zA-Z\s\-]+$"))
            return Result<CustomerInfo>.Failure("Full name can only contain letters, spaces, and hyphens.");

        if (string.IsNullOrWhiteSpace(email) || email.Length > 255 || !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            return Result<CustomerInfo>.Failure("Invalid email format.");

        if (string.IsNullOrWhiteSpace(phoneNumber) || !Regex.IsMatch(phoneNumber, @"^\+[1-9]\d{7,14}$"))
            return Result<CustomerInfo>.Failure("Phone number must be in international format and contain 8 to 15 digits.");

        if (string.IsNullOrWhiteSpace(nationality) || nationality.Length != 2)
            return Result<CustomerInfo>.Failure("Nationality must be a valid ISO 3166-1 alpha-2 country code.");

        if (!isDomestic && string.IsNullOrWhiteSpace(passportNumber))
            return Result<CustomerInfo>.Failure("Passport number is required for international customers.");

        if (!string.IsNullOrWhiteSpace(passportNumber) && (passportNumber.Length < 6 || passportNumber.Length > 12 || !Regex.IsMatch(passportNumber, @"^[a-zA-Z0-9]+$")))
            return Result<CustomerInfo>.Failure("Passport number must be 6-12 alphanumeric characters.");

        return Result<CustomerInfo>.Success(new CustomerInfo(fullName, email, phoneNumber, nationality, passportNumber));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return FullName;
        yield return Email;
        yield return PhoneNumber;
        yield return Nationality;
        if (PassportNumber != null) yield return PassportNumber;
    }
}