# C4 Container

## Purpose

This document describes the major deployable/runtime containers inside the current solution.

## Containers

- `Reservations.API`
  - Main ASP.NET Core host.
  - Composes Reservations, Availability, Pricing, Notifications, Users, and Audit services.
  - Hosts HTTP endpoints, event subscriptions, outbox processing, logging, and middleware.
- `Availability.API`
  - Lightweight module-specific API host.
  - Exposes availability endpoints.
- `SQL Server`
  - Persists Reservations data and Outbox messages.
- `RabbitMQ`
  - Carries integration events between modules.

## Module-Level Responsibilities Inside `Reservations.API`

- Reservations
  - Reservation aggregate, commands, queries, persistence, outbox.
- Availability
  - Availability blocking/release logic and subscription to reservation-created events.
- Pricing
  - Price calculation and subscription to reservation-created events.
- Notifications
  - Sends reservation-related notification messages.
- Users
  - User registration and basic authentication support.
- Audit
  - Stores integration-event audit records.
- BuildingBlocks
  - Shared kernel, event bus, messaging, observability.

## PlantUML

```plantuml
@startuml
!include https://raw.githubusercontent.com/plantuml-stdlib/C4-PlantUML/master/C4_Container.puml

Person(customer, "Customer")

System_Boundary(system, "EventDrivenBookingPlatform") {
  Container(resApi, "Reservations.API", "ASP.NET Core", "Main application host and composition root")
  Container(avApi, "Availability.API", "ASP.NET Core", "Availability-focused API host")
}

ContainerDb(sql, "SQL Server", "SQL Server", "Reservations persistence and outbox")
ContainerQueue(rabbit, "RabbitMQ", "RabbitMQ", "Integration event transport")
System_Ext(email, "Email Provider", "Fake/SMTP provider")

Rel(customer, resApi, "Uses", "HTTPS")
Rel(customer, avApi, "Uses", "HTTPS")
Rel(resApi, sql, "Reads/writes", "EF Core")
Rel(resApi, rabbit, "Publishes/consumes", "AMQP")
Rel(resApi, email, "Sends notifications", "SMTP/API")
Rel(avApi, rabbit, "Consumes/publishes when needed", "AMQP")

@enduml
```

## Notes

- Most runtime orchestration currently happens in `Reservations.API`.
- Some modules are intentionally implemented with in-memory infrastructure at this stage and can later move to dedicated persistence.
