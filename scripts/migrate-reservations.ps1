param(
    [string]$StartupProject = "src/Modules/Reservations/API/API.csproj",
    [string]$Project = "src/Modules/Reservations/Infrastructure/Infrastructure.csproj"
)

$ErrorActionPreference = "Stop"

dotnet ef database update `
    --project $Project `
    --startup-project $StartupProject
