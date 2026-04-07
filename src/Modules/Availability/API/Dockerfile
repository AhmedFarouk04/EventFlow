FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["EventDrivenBookingPlatform.sln", "./"]
COPY ["src/BuildingBlocks/BuildingBlocks.csproj", "src/BuildingBlocks/"]
COPY ["src/Modules/Availability/API/Availability.API.csproj", "src/Modules/Availability/API/"]
COPY ["src/Modules/Availability/Application/Availability.Application.csproj", "src/Modules/Availability/Application/"]
COPY ["src/Modules/Availability/Contracts/Availability.Contracts.csproj", "src/Modules/Availability/Contracts/"]
COPY ["src/Modules/Availability/Domain/Availability.Domain.csproj", "src/Modules/Availability/Domain/"]
COPY ["src/Modules/Availability/Infrastructure/Availability.Infrastructure.csproj", "src/Modules/Availability/Infrastructure/"]
COPY ["src/Modules/Reservations/Contracts/Contracts.csproj", "src/Modules/Reservations/Contracts/"]
COPY ["src/Modules/Pricing/Contracts/Pricing.Contracts.csproj", "src/Modules/Pricing/Contracts/"]

RUN dotnet restore "src/Modules/Availability/API/Availability.API.csproj"

COPY . .
WORKDIR /src/src/Modules/Availability/API
RUN dotnet publish "Availability.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8081
ENTRYPOINT ["dotnet", "Availability.API.dll"]
