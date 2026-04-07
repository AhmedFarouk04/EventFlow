# ADR-002 Event Driven Internal Communication

## Status

Accepted

## Context

Supporting modules should react to reservation lifecycle changes without tightly coupling direct method calls across modules.

## Decision

Use internal integration events published through RabbitMQ abstractions:

- Reservations publishes integration events
- Availability, Pricing, Notifications, and Audit subscribe to those events
- module contracts define event payloads

## Consequences

- modules remain loosely coupled
- workflows become easier to extend
- operational tracing becomes more important
- eventual consistency must be accepted and tested
