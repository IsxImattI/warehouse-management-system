# Warehouse Management System (WMS)

An open-source, mobile-first warehouse management system built for small and medium businesses. Designed to integrate with existing ERP systems via API.

Built with ASP.NET Core · SQL Server · SvelteKit

---

## Features

- Item catalog management (SKU, EAN)
- Location/bin management with QR code support
- Inventory movement tracking with full transaction safety
- Complete movement history log
- Mobile-first PWA frontend with barcode/QR scanning
- Scan-to-receive mobile flow
- ERP integration ready via REST API

---

## Tech Stack

| Layer    | Technology                  |
|----------|-----------------------------|
| Backend  | ASP.NET Core 10, EF Core    |
| Database | SQL Server / LocalDB        |
| Frontend | SvelteKit 2, Tailwind CSS v4|
| Scanning | html5-qrcode                |
| Testing  | xUnit                       |
| CI/CD    | GitHub Actions              |

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server or LocalDB
- Node.js 20+

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
   Edit `.env` and set your connection string:
```
   CONNECTIONSTRINGS__DEFAULT=Server=(localdb)\MSSQLLocalDB;Database=WMS_Dev;Trusted_Connection=True;TrustServerCertificate=True
```

3. Apply database migrations:
```bash
   dotnet ef database update
```

4. Run the API:
```bash
   dotnet run
```

5. Open Swagger UI at `https://localhost:{port}/swagger`

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

4. For mobile testing (requires ngrok):
```bash
   npm run dev -- --host
   ngrok http 5173
```

### Running Tests

```bash
cd backend
dotnet test
```

---

## API Endpoints

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
| Method | Endpoint        | Description            |
|--------|-----------------|------------------------|
| POST   | /api/movement   | Execute stock movement |

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
