param(
    [string]$StartupProject = "src/Modules/Availability/API/Availability.API.csproj",
    [string]$Project = "src/Modules/Availability/Infrastructure/Availability.Infrastructure.csproj",
    [string]$Context = "EventDrivenBookingPlatform.Modules.Availability.Infrastructure.Persistence.AvailabilityDbContext"
)

$ErrorActionPreference = "Stop"

dotnet ef database update `
    --project $Project `
    --startup-project $StartupProject `
    --context $Context
