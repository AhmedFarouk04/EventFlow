param(
    [string]$StartupProject = "src/Modules/Reservations/API/API.csproj",
    [string]$Project = "src/Modules/Notifications/Infrastructure/Notifications.Infrastructure.csproj",
    [string]$Context = "EventDrivenBookingPlatform.Modules.Notifications.Infrastructure.Persistence.NotificationsDbContext"
)

$ErrorActionPreference = "Stop"

dotnet ef database update `
    --project $Project `
    --startup-project $StartupProject `
    --context $Context
