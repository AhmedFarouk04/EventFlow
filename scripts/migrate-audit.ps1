param(
    [string]$StartupProject = "src/Modules/Reservations/API/API.csproj",
    [string]$Project = "src/Modules/Audit/Infrastructure/Audit.Infrastructure.csproj",
    [string]$Context = "EventDrivenBookingPlatform.Modules.Audit.Infrastructure.Persistence.AuditDbContext"
)

$ErrorActionPreference = "Stop"

dotnet ef database update `
    --project $Project `
    --startup-project $StartupProject `
    --context $Context
