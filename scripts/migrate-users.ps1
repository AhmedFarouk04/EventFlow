param(
    [string]$StartupProject = "src/Modules/Reservations/API/API.csproj",
    [string]$Project = "src/Modules/Users/Infrastructure/Users.Infrastructure.csproj",
    [string]$Context = "EventDrivenBookingPlatform.Modules.Users.Infrastructure.Persistence.UsersDbContext"
)

$ErrorActionPreference = "Stop"

dotnet ef database update `
    --project $Project `
    --startup-project $StartupProject `
    --context $Context
