# ADR-004 Outbox Pattern

## Status

Accepted

## Context

Reservation state changes and event publication must stay consistent. Directly publishing to RabbitMQ inside command handlers would create a dual-write risk.

## Decision

Use the Outbox pattern:

- domain events are converted to integration events inside persistence
- integration events are stored in `OutboxMessages`
- `OutboxProcessor` publishes them asynchronously

## Consequences

- reduced dual-write risk
- support for retries and failure tracking
- additional infrastructure complexity
- eventual consistency instead of immediate dispatch
