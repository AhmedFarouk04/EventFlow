FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["EventDrivenBookingPlatform.sln", "./"]
COPY ["src/BuildingBlocks/BuildingBlocks.csproj", "src/BuildingBlocks/"]
COPY ["src/Modules/Reservations/API/API.csproj", "src/Modules/Reservations/API/"]
COPY ["src/Modules/Reservations/Application/Application.csproj", "src/Modules/Reservations/Application/"]
COPY ["src/Modules/Reservations/Contracts/Contracts.csproj", "src/Modules/Reservations/Contracts/"]
COPY ["src/Modules/Reservations/Domain/Domain.csproj", "src/Modules/Reservations/Domain/"]
COPY ["src/Modules/Reservations/Infrastructure/Infrastructure.csproj", "src/Modules/Reservations/Infrastructure/"]
COPY ["src/Modules/Availability/Application/Availability.Application.csproj", "src/Modules/Availability/Application/"]
COPY ["src/Modules/Availability/Infrastructure/Availability.Infrastructure.csproj", "src/Modules/Availability/Infrastructure/"]
COPY ["src/Modules/Pricing/Application/Pricing.Application.csproj", "src/Modules/Pricing/Application/"]
COPY ["src/Modules/Pricing/Contracts/Pricing.Contracts.csproj", "src/Modules/Pricing/Contracts/"]
COPY ["src/Modules/Pricing/Domain/Pricing.Domain.csproj", "src/Modules/Pricing/Domain/"]
COPY ["src/Modules/Notifications/Application/Notifications.Application.csproj", "src/Modules/Notifications/Application/"]
COPY ["src/Modules/Notifications/Infrastructure/Notifications.Infrastructure.csproj", "src/Modules/Notifications/Infrastructure/"]
COPY ["src/Modules/Users/Application/Users.Application.csproj", "src/Modules/Users/Application/"]
COPY ["src/Modules/Users/Domain/Users.Domain.csproj", "src/Modules/Users/Domain/"]
COPY ["src/Modules/Users/Infrastructure/Users.Infrastructure.csproj", "src/Modules/Users/Infrastructure/"]
COPY ["src/Modules/Audit/Application/Audit.Application.csproj", "src/Modules/Audit/Application/"]
COPY ["src/Modules/Audit/Infrastructure/Audit.Infrastructure.csproj", "src/Modules/Audit/Infrastructure/"]

RUN dotnet restore "src/Modules/Reservations/API/API.csproj"

COPY . .
WORKDIR /src/src/Modules/Reservations/API
RUN dotnet publish "API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "API.dll"]
