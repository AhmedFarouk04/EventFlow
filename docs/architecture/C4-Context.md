# C4 Context

## Purpose

This document describes the system at the Context level for the `EventDrivenBookingPlatform` solution.

## Scope

The platform is implemented as a modular monolith in .NET 8. The current solution hosts reservation-centric workflows and supporting modules inside the same codebase, with RabbitMQ used for integration-style internal messaging and SQL Server used for persistent storage where implemented.

## People And External Systems

- Customer
  - Creates and manages reservations through HTTP APIs.
- Operator / Support
  - Reads reservation and availability data and can validate operational state.
- EventDrivenBookingPlatform
  - Core business system that manages reservations, availability, pricing, notifications, users, and audit logs.
- SQL Server
  - Stores reservation data and outbox records.
- RabbitMQ
  - Moves integration events between modules.
- Email Provider
  - Represented currently by a fake email implementation in the Notifications module.

## PlantUML

```plantuml
@startuml
!include https://raw.githubusercontent.com/plantuml-stdlib/C4-PlantUML/master/C4_Context.puml

Person(customer, "Customer", "Creates and manages reservations.")
Person(operator, "Operator", "Reads operational reservation data.")

System(system, "EventDrivenBookingPlatform", "Modular monolith for reservations and supporting workflows.")
System_Ext(sql, "SQL Server", "Stores persistent module data.")
System_Ext(rabbit, "RabbitMQ", "Transports integration events.")
System_Ext(email, "Email Provider", "Sends customer-facing notifications.")

Rel(customer, system, "Uses", "HTTPS/JSON")
Rel(operator, system, "Operates", "HTTPS/JSON")
Rel(system, sql, "Reads/writes reservation data", "EF Core")
Rel(system, rabbit, "Publishes and consumes events", "AMQP")
Rel(system, email, "Sends notification requests", "SMTP/API (planned), fake service currently")

@enduml
```

## Notes

- The current host entry point is `Reservations.API`, which wires the other modules into the runtime.
- `Availability.API` also exists as a separate lightweight host in the solution.
