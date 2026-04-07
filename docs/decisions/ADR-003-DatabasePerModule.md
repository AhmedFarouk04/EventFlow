# ADR-003 Database Per Module Boundary

## Status

Accepted

## Context

The solution should evolve toward independent persistence boundaries even while running as a modular monolith.

## Decision

Model persistence per module boundary:

- Reservations currently owns SQL persistence
- other modules are allowed to start with in-memory or minimal implementations
- future persistence should remain module-owned instead of creating a shared domain database model

## Consequences

- stronger ownership boundaries
- easier future extraction to separate services
- duplicated infrastructure code may appear and should be managed intentionally
