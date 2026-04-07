# Code Review

## Completed In This Pass

- Fixed availability blocking so reservations spanning multiple days block each affected day instead of only the first day.
- Hardened `RabbitMqEventBus` subscription storage to avoid mutating shared `List<Type>` instances concurrently.
- Extended integration coverage to assert multi-day availability blocking and downstream event side effects.

## Residual Risks

- Several supporting modules still use in-memory infrastructure and should move to persistent storage before production use.
- Performance coverage currently relies on a lightweight k6 script rather than a broader benchmark matrix.
- The main runtime composition still happens in `Reservations.API`; a more explicit host/bootstrap strategy may be desirable as the system grows.
