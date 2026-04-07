import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
  vus: 10,
  duration: '30s',
  thresholds: {
    http_req_failed: ['rate<0.01'],
    http_req_duration: ['p(95)<800']
  }
};

export default function () {
  const start = new Date();
  start.setDate(start.getDate() + 7);

  const end = new Date(start);
  end.setDate(end.getDate() + 2);

  const payload = JSON.stringify({
    customerId: crypto.randomUUID(),
    serviceId: '11111111-1111-1111-1111-111111111111',
    startDate: start.toISOString(),
    endDate: end.toISOString(),
    items: [
      { name: 'Room', quantity: 1, unitPrice: 150.0 }
    ]
  });

  const params = {
    headers: {
      'Content-Type': 'application/json'
    }
  };

  const response = http.post('http://localhost:8080/api/reservations', payload, params);

  check(response, {
    'status is 201': (r) => r.status === 201
  });

  sleep(1);
}
