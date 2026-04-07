# Performance

## k6 Load Test

The repository includes a basic load script for the reservation creation endpoint:

- `tests/performance/reservations-create-load.js`

Run it after starting the API locally or with Docker:

```powershell
k6 run tests/performance/reservations-create-load.js
```

## Current Focus

- reservation creation latency
- basic request failure rate
- outbox-backed event publication path under concurrent create requests

## Notes

- This is a lightweight baseline, not a full benchmark suite.
- If you run against Docker Compose, use `http://localhost:8080`.
