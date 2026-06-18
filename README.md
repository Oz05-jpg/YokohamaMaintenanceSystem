# Yokohama Maintenance System

Equipment maintenance management system for manufacturing environments, built with ASP.NET Core MVC and .NET 10.

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Language | C# (.NET 10) |
| Framework | ASP.NET Core MVC |
| ORM | Entity Framework Core |
| Database | SQL Server |
| Auth | ASP.NET Core Identity + Role-based Authorization |
| Testing | xUnit + Moq |
| API | ASP.NET Core Web API + Swagger |

## Features

- **Machine Management** — CRUD for factory machines with role-based access
- **Maintenance Requests** — Create, track, and update repair/maintenance tickets
- **Technician Assignment** — Assign technicians to requests via dropdown (FK relationship)
- **Status Tracking** — Enum-based status (Pending / InProgress / Completed) with color badges
- **Report Dashboard** — Summary counts grouped by status and machine using ViewModel + LINQ
- **Search & Filter** — IQueryable-based filtering with conditional LINQ (no extra DB round-trips)
- **Pagination** — Skip/Take with OrderBy and PagedRequestViewModel
- **Repository Pattern** — Interface-based data access layer with Dependency Injection
- **Unit Testing** — xUnit + Moq + InMemory DB (18 tests)
- **Global Error Handling** — UseExceptionHandler + UseStatusCodePagesWithReExecute + ILogger
- **REST API** — JSON endpoints for external integration (GET / PUT status) + Swagger UI

## Project Structure

```
YokohamaMaintenanceSystem/
├── Controllers/          # MVC + API Controllers
├── Interfaces/           # Repository interfaces
├── Repositories/         # EF Core data access implementations
├── Models/               # Domain models + ViewModels + DTOs
├── Views/                # Razor views (.cshtml)
├── Data/                 # AppDbContext + DbInitializer
├── Enums/                # RequestStatus enum
├── Areas/Identity/       # ASP.NET Core Identity pages
└── YokohamaMaintenanceSystem.Tests/  # xUnit test project
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

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/maintenance` | Get all maintenance requests |
| GET | `/api/maintenance/{id}` | Get request by ID |
| PUT | `/api/maintenance/{id}/status` | Update request status |

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
