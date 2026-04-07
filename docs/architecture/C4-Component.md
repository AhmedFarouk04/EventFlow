# C4 Component

## Purpose

This document describes the component view of the main `Reservations.API` host.

## Main Components

- Controllers
  - `ReservationsController`
  - `AvailabilityController`
- Middleware
  - Exception handling
  - Correlation ID
  - Serilog request logging
- Application Layer
  - CQRS handlers and validators
  - Module event handlers
- Domain Layer
  - Aggregates, value objects, domain events, business rules
- Infrastructure Layer
  - EF Core DbContext
  - Repositories
  - Outbox storage
  - Event publisher integrations
- BuildingBlocks
  - EventBus
  - Outbox/Inbox/Idempotency
  - Observability

## Event Flow

1. Client sends `POST /api/reservations`.
2. `ReservationsController` dispatches `CreateReservationCommand`.
3. Reservation aggregate raises `ReservationCreatedEvent`.
4. `ReservationsDbContext` maps the domain event to an integration event and stores it in the outbox.
5. `OutboxProcessor` publishes the integration event to RabbitMQ.
6. Availability, Pricing, Notifications, and Audit consumers react to the event.

## PlantUML

```plantuml
@startuml
!include https://raw.githubusercontent.com/plantuml-stdlib/C4-PlantUML/master/C4_Component.puml

Container_Boundary(api, "Reservations.API") {
  Component(controller, "Controllers", "ASP.NET Core MVC", "HTTP endpoints")
  Component(middleware, "Middleware", "ASP.NET Core", "Error handling, correlation, request logging")
  Component(app, "Application Handlers", "MediatR", "Commands, queries, validators, module event handlers")
  Component(domain, "Domain Model", ".NET classes", "Aggregates, value objects, rules, domain events")
  Component(infra, "Infrastructure", "EF Core + messaging adapters", "Repositories, DbContext, OutboxStore")
  Component(bb, "BuildingBlocks", ".NET libraries", "Event bus, messaging, observability")
}

ComponentDb(sql, "ReservationsDbContext", "EF Core", "Reservation and outbox persistence")
ComponentQueue(rabbit, "RabbitMQ Exchange/Queue", "RabbitMQ", "Internal event transport")

Rel(controller, middleware, "Runs through")
Rel(controller, app, "Dispatches requests to")
Rel(app, domain, "Executes business rules in")
Rel(app, infra, "Uses repositories from")
Rel(infra, sql, "Reads/writes")
Rel(infra, bb, "Uses")
Rel(bb, rabbit, "Publishes/consumes")

@enduml
```

## Notes

- The current component split follows Clean Architecture boundaries closely in the Reservations module.
- Supporting modules are integrated through application-layer event handlers.
