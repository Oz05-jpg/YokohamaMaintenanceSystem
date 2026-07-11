# Yokohama Maintenance System

Equipment maintenance management system for manufacturing environments, built with ASP.NET Core MVC and .NET 10 — developed ticket-by-ticket as a portfolio project simulating a real maintenance dept. workflow.

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Language | C# (.NET 10) |
| Framework | ASP.NET Core MVC |
| ORM | Entity Framework Core |
| Database | SQL Server |
| Auth (Web) | ASP.NET Core Identity + Role-based Authorization |
| Auth (API) | JWT Bearer + Refresh Token rotation |
| Real-time | SignalR |
| Reporting | ClosedXML (Excel) · QuestPDF (PDF) · CSV |
| Testing | xUnit + Moq + WebApplicationFactory (integration) |
| CI/CD | GitHub Actions |
| API Docs | Swagger / OpenAPI |

## Architecture & Design Patterns

Built to demonstrate applied OOP/SOLID, not just CRUD:

- **Repository Pattern (Generic)** — `IRepository<T>` / `Repository<T>` base class; `MachineRepository`, `TechnicianRepository`, `MaintenanceRequestRepository` extend it instead of duplicating CRUD
- **Factory Pattern** — `ReportExporterFactory` picks `CsvExporter` / `ExcelExporter` / `PdfExporter` behind a shared `IReportExporter` interface
- **Strategy Pattern** — `INotificationStrategy` (`LogNotificationStrategy`, `SignalRNotificationStrategy`) swapped at runtime for overdue-request alerts
- **Singleton** — `AuditLogService` registered as a thread-safe singleton (`lock` + defensive copy over the log list)
- **Observer (Delegates/Events)** — `MaintenanceNotifier.StatusChanged` event; `AuditLogService` subscribes and logs every status change without the notifier knowing who's listening
- **Dependency Injection** — constructor injection throughout controllers, services, and repositories

## Features

**Core CRUD**
- Machine Management, Maintenance Request tracking (status workflow), Technician CRUD + assignment (FK dropdown)

**Auth & Access**
- ASP.NET Core Identity + role-based Admin panel (MVC)
- JWT Bearer + Refresh Token rotation for the Web API (`AuthController`)

**Real-time & Background**
- SignalR live alerts (`MaintenanceHub`)
- `OverdueRequestAlertService` — background job (`IHostedService`) polling for overdue requests on a timer

**Reporting & Export**
- Report dashboard (status/machine summary via ViewModel + LINQ)
- CSV / Excel / PDF export (Factory pattern above)

**Data & Quality**
- Search, Filter, Pagination (`IQueryable`, no extra DB round-trips)
- Global Error Handling (`UseExceptionHandler` + `UseStatusCodePagesWithReExecute`) + `ILogger`
- Audit Log (who did what, when) exposed via `GET /api/audit-logs`
- Unit + integration test suite (xUnit, Moq, in-memory DB, `WebApplicationFactory`)
- CI pipeline (GitHub Actions) running build + tests on push

## Project Structure

```
YokohamaMaintenanceSystem/
├── Controllers/          # MVC + API Controllers
├── Interfaces/           # Repository/service interfaces
├── Repositories/         # Generic + entity-specific EF Core data access
├── Services/             # AuditLogService, MaintenanceNotifier, notification strategies, background alert service
├── Factories/            # ReportExporterFactory
├── Exporters/            # Csv/Excel/PdfExporter implementations
├── Hubs/                 # SignalR MaintenanceHub
├── Models/               # Domain models + ViewModels
├── DTOs/                 # API request/response DTOs (e.g. LoginDto)
├── Views/                # Razor views (.cshtml)
├── Data/                 # AppDbContext + DbInitializer
├── Enums/                # RequestStatus enum
├── Areas/Identity/       # ASP.NET Core Identity pages
├── .github/workflows/    # CI pipeline (ci.yml)
└── YokohamaMaintenanceSystem.Tests/  # xUnit + integration test project
```

## Getting Started

### Prerequisites
- Visual Studio 2022
- .NET 10 SDK
- SQL Server (LocalDB or full)

### Setup

1. Clone the repository
```bash
git clone https://github.com/Oz05-jpg/YokohamaMaintenanceSystem.git
```

2. Update connection string in `appsettings.json`
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=YokohamaMaintenanceDb;Trusted_Connection=True;"
}
```

3. Apply migrations
```bash
dotnet ef database update
```

4. Run the project
```bash
dotnet run
```

5. Open Swagger UI at `https://localhost:{port}/swagger`

## API Endpoints

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| POST | `/api/auth/login` | Log in, returns JWT + refresh token | — |
| POST | `/api/auth/refresh` | Rotate refresh token | — |
| POST | `/api/auth/logout` | Revoke refresh token | Bearer |
| GET | `/api/maintenance` | Get all maintenance requests | Bearer |
| GET | `/api/maintenance/{id}` | Get request by ID | Bearer |
| PUT | `/api/maintenance/{id}/status` | Update request status | Bearer |
| GET | `/api/audit-logs` | Get audit log entries | Bearer |

Swagger UI: `https://localhost:{port}/swagger`

## Sprint Tickets

| Ticket | Feature | Status |
|--------|---------|--------|
| #001 | Machine CRUD + Identity Setup | ✅ |
| #002 | MaintenanceRequest CRUD | ✅ |
| #003 | Technician CRUD + Industrial UI | ✅ |
| #004 | SelectList + FK Dropdown | ✅ |
| #005 | RequestStatus Enum + Badge + UpdateStatus | ✅ |
| #006 | Report Page (ViewModel + LINQ Count) | ✅ |
| #007 | Repository Pattern (Interface + DI) | ✅ |
| #008 | Unit Testing (xUnit + Moq) | ✅ |
| #009 | Search & Filter (IQueryable) | ✅ |
| #010 | Pagination (Skip/Take) | ✅ |
| #011 | Global Error Handling + ILogger | ✅ |
| #012 | Web API + Swagger | ✅ |
| #013 | JWT Authentication for Web API | ✅ |
| #014 | Refresh Token + Token Rotation + Logout | ✅ |
| #015 | BackgroundService (overdue request check) | ✅ |
| #016 | IMemoryCache Dashboard Cache | ✅ |
| #017 | SignalR Real-time Alert | ✅ |
| #018 | Excel Export (ClosedXML) | ✅ |
| #019 | GitHub Actions CI/CD Pipeline | ✅ |
| #020 | Integration Testing (WebApplicationFactory) | ✅ |
| #022 | Factory Pattern (Report Exporter) | ✅ |
| #023 | PDF Export (QuestPDF) | ✅ |
| #024 | Singleton Pattern (AuditLogService) | ✅ |
| #025 | Thread-Safe AuditLogService | ✅ |
| #026 | Delegates/Events (Status Change Notifier) | ✅ |
| #027 | Wire StatusChanged → AuditLogService subscriber | ✅ |
| #028 | Generic Repository (`IRepository<T>`/`Repository<T>`) | ✅ |
| #029–031 | Repository<T> reuse (Machine, Technician) + Strategy Pattern (overdue notifications) | ✅ |

*(#021 Docker containerization — parked, not required by target JD)*
