param(
    [string]$StartupProject = "src/Modules/Reservations/API/API.csproj",
    [string]$Project = "src/Modules/Pricing/Infrastructure/Pricing.Infrastructure.csproj",
    [string]$Context = "EventDrivenBookingPlatform.Modules.Pricing.Infrastructure.Persistence.PricingDbContext"
)

$ErrorActionPreference = "Stop"

dotnet ef database update `
    --project $Project `
    --startup-project $StartupProject `
    --context $Context
