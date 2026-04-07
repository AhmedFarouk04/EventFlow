# EventDrivenBookingPlatform

`EventDrivenBookingPlatform` is a .NET 8 modular monolith that implements reservation workflows with supporting modules for availability, pricing, notifications, users, and audit.

## Solution Overview

Current solution layout:

- `src/BuildingBlocks`
- `src/Modules/Reservations`
- `src/Modules/Availability`
- `src/Modules/Pricing`
- `src/Modules/Notifications`
- `src/Modules/Users`
- `src/Modules/Audit`
- `tests`

The main runtime host is `src/Modules/Reservations/API`. A lightweight `Availability.API` host also exists in the solution.

## Architecture

The codebase follows a modular clean architecture style:

- `Domain`
  - aggregates, value objects, rules, domain events
- `Application`
  - commands, queries, handlers, validators, event handlers
- `Infrastructure`
  - persistence, repositories, messaging adapters
- `API`
  - ASP.NET Core host, middleware, DI, controllers
- `Contracts`
  - integration events used across modules

Shared cross-cutting concerns live in `src/BuildingBlocks`:

- `SharedKernel`
- `EventBus`
- `Messaging`
- `Observability`

## Event Flow

The current reservation flow works like this:

1. A client creates a reservation through `Reservations.API`.
2. The Reservations domain raises a domain event.
3. The event is persisted to the Outbox.
4. `OutboxProcessor` publishes an integration event to RabbitMQ.
5. Availability, Pricing, Notifications, and Audit consume the event.

## Running Locally

### Option 1: Visual Studio / dotnet

Build:

```powershell
dotnet build EventDrivenBookingPlatform.sln
```

Run tests:

```powershell
dotnet test EventDrivenBookingPlatform.sln
```

Run reservations host:

```powershell
dotnet run --project src/Modules/Reservations/API/API.csproj
```

Run availability host:

```powershell
dotnet run --project src/Modules/Availability/API/Availability.API.csproj
```

### Option 2: Docker Compose

The repository includes:

- SQL Server
- RabbitMQ with management UI
- Reservations API
- Availability API
- Reservations migration runner
- Availability migration runner
- Audit migration runner
- Pricing migration runner
- Notifications migration runner
- Users migration runner

Start:

```powershell
docker compose -f docker/docker-compose.yml up --build
```

Endpoints:

- Reservations API: `http://localhost:8080`
- Availability API: `http://localhost:8081`
- RabbitMQ management: `http://localhost:15672`

## Migrations

Reservations migrations are applied through either:

```powershell
./scripts/migrate-reservations.ps1
```

Availability migrations are applied through:

```powershell
./scripts/migrate-availability.ps1
```

Audit migrations are applied through:

```powershell
./scripts/migrate-audit.ps1
```

Pricing migrations are applied through:

```powershell
./scripts/migrate-pricing.ps1
```

Notifications migrations are applied through:

```powershell
./scripts/migrate-notifications.ps1
```

Users migrations are applied through:

```powershell
./scripts/migrate-users.ps1
```

or the migration services in Docker Compose.

## Testing

Current test projects:

- `tests/Availability.IntegrationTests`
- `tests/Reservations.UnitTests`
- `tests/Reservations.IntegrationTests`
- `tests/Architecture.Tests`
- `tests/EventHandling.Tests`

Run all tests:

```powershell
dotnet test EventDrivenBookingPlatform.sln
```

## Documentation

Additional architecture documentation lives in:

- `docs/architecture`
- `docs/decisions`
- `docs/performance`
- `docs/reviews`

## Development Notes

- Reservations currently has the deepest production-style implementation.
- Some supporting modules still use in-memory infrastructure as an intentional intermediate step.
- Architecture and event tests are included to protect module boundaries and messaging behavior while the solution evolves.
- A baseline k6 script is available at `tests/performance/reservations-create-load.js`.
