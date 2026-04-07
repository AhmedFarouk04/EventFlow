# ADR-001 Modular Monolith

## Status

Accepted

## Context

The platform needs multiple business modules such as Reservations, Availability, Pricing, Notifications, Users, and Audit, but the implementation should stay operationally simple during early development.

## Decision

Use a modular monolith structure:

- each module has its own projects and boundaries
- the system is deployed from a shared solution
- module interactions use contracts and event-driven messaging patterns

## Consequences

- simpler deployment than microservices
- good internal separation for later extraction
- lower operational overhead in early stages
- stronger need for architecture tests to protect boundaries
