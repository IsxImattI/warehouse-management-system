# Warehouse Management System (WMS)

An open-source, mobile-first warehouse management system built for small and medium businesses. Designed to integrate with existing ERP systems via API.

Built with ASP.NET Core · SQL Server · SvelteKit

---

## Features

- Item catalog management (SKU, EAN, barcode scanning)
- Location/bin management with QR code generation and printing
- Inventory movement tracking with full transaction safety
- Complete movement history log with user attribution
- Mobile-first PWA frontend with barcode/QR scanning
- Scan-to-receive mobile flow
- JWT authentication with role-based access (admin / operator)
- User management (admin only)
- ERP integration ready via REST API

---

## Tech Stack

| Layer      | Technology                   |
|------------|------------------------------|
| Backend    | ASP.NET Core 10, EF Core     |
| Database   | SQL Server / LocalDB         |
| Frontend   | SvelteKit 2, Tailwind CSS v4 |
| Scanning   | html5-qrcode                 |
| Auth       | JWT Bearer tokens, BCrypt    |
| Testing    | xUnit (11 tests)             |
| CI/CD      | GitHub Actions               |

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server or LocalDB
- Node.js 20+
- [dotnet-ef](https://learn.microsoft.com/en-us/ef/core/cli/dotnet) CLI tool

```bash
dotnet tool install --global dotnet-ef
```

### Backend Setup

1. Clone the repo:
```bash
   git clone https://github.com/lsxlmattl/warehouse-management-system.git
   cd warehouse-management-system/backend/WMS.Api
```

2. Create your `.env` file:
```bash
   cp .env.example .env
```
   Edit `.env` and set your values:
```
   CONNECTIONSTRINGS__DEFAULT=Server=(localdb)\MSSQLLocalDB;Database=WMS_Dev;Trusted_Connection=True;TrustServerCertificate=True
   JWT__SECRET=your-super-secret-key-change-this-in-production-min-32-chars
   JWT__ISSUER=wms-api
   JWT__AUDIENCE=wms-client
```

3. Apply database migrations:
```bash
   dotnet ef database update
```

4. Run the API:
```bash
   dotnet run
```

5. Open Swagger UI at `http://localhost:{port}/swagger`

6. Create your first admin user via Swagger — `POST /api/auth/register`:
```json
   {
     "username": "admin",
     "password": "yourpassword"
   }
```
   Then update the role directly in the database:
```sql
   UPDATE Users SET Role = 'admin' WHERE Username = 'admin';
```

### Frontend Setup

1. Install dependencies:
```bash
   cd frontend
   npm install
```

2. Create `.env` file:
```bash
   cp .env.example .env
```
   Set your backend URL:
```
   VITE_API_URL=http://localhost:5079
```

3. Start development server:
```bash
   npm run dev
```

4. For mobile testing with QR/barcode scanning (requires HTTPS via ngrok):
```bash
   npm run dev -- --host
   ngrok http 5173
```
   Add the ngrok host to `vite.config.ts`:
```typescript
   server: {
     allowedHosts: ['your-ngrok-subdomain.ngrok-free.app']
   }
```

### Running Tests

```bash
cd backend
dotnet test
```

---

## API Endpoints

### Auth
| Method | Endpoint              | Description        |
|--------|-----------------------|--------------------|
| POST   | /api/auth/register    | Register user      |
| POST   | /api/auth/login       | Login, get token   |

### Items
| Method | Endpoint         | Description       |
|--------|------------------|-------------------|
| GET    | /api/items       | Get all items     |
| GET    | /api/items/{id}  | Get item by ID    |
| POST   | /api/items       | Create item       |
| PUT    | /api/items/{id}  | Update item       |
| DELETE | /api/items/{id}  | Delete item       |

### Locations
| Method | Endpoint              | Description          |
|--------|-----------------------|----------------------|
| GET    | /api/locations        | Get all locations    |
| GET    | /api/locations/{id}   | Get location by ID   |
| POST   | /api/locations        | Create location      |
| PUT    | /api/locations/{id}   | Update location      |
| DELETE | /api/locations/{id}   | Delete location      |

### Inventory
| Method | Endpoint                        | Description               |
|--------|---------------------------------|---------------------------|
| GET    | /api/inventory                  | Get full inventory        |
| GET    | /api/inventory/location/{id}    | Get inventory by location |
| GET    | /api/inventory/item/{id}        | Get inventory by item     |

### Movement
| Method | Endpoint             | Description            |
|--------|----------------------|------------------------|
| POST   | /api/movement        | Execute stock movement |
| GET    | /api/movementlogs    | Get movement history   |

### Users (admin only)
| Method | Endpoint          | Description       |
|--------|-------------------|-------------------|
| GET    | /api/users        | Get all users     |
| POST   | /api/users        | Create user       |
| DELETE | /api/users/{id}   | Delete user       |

#### Movement Request Body
```json
{
  "itemId": 1,
  "fromLocationId": null,
  "toLocationId": 1,
  "quantity": 10,
  "note": "Initial stock entry"
}
```

> Set `fromLocationId` to `null` for incoming stock, `toLocationId` to `null` for outgoing.

---

## Sponsorship

This project is free and open-source under AGPLv3. If WMS saves you time or money, consider supporting its development.

[Become a sponsor on GitHub](https://github.com/sponsors/lsxlmattl)

---

## Contributing

Contributions are welcome. Please read [CONTRIBUTING.md](CONTRIBUTING.md) before submitting a PR.

1. Fork the repo
2. Create a feature branch: `git checkout -b feature/your-feature`
3. Commit using [Conventional Commits](https://www.conventionalcommits.org/): `feat: add your feature`
4. Push and open a Pull Request against `dev`

All PRs require passing CI checks and review approval from the maintainer.

---

## License

Licensed under [AGPLv3](LICENSE). For commercial use without open-source obligations, contact us for a commercial license.

---

## Author

Built by [lsxlmattl](https://github.com/lsxlmattl) · Matej Gyergyek