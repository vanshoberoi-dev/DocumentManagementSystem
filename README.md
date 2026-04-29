# Document Management System (DMS)
A web-based document management system built as part of my 3rd semester assignment.
Built using ASP.NET Core MVC with .NET 8.

---

## What it does
- Upload and manage documents (PDFs, images, word files etc)
- Maintains version history when you re-upload a document
- Tag documents and filter by tags
- Search documents by title or description
- Dashboard showing stats like total docs, storage used, recently uploaded
- Full audit log of who did what and when
- Admin and User roles with different permissions

---

## Tech Stack
- ASP.NET Core MVC (.NET 8)
- Entity Framework Core (Code First)
- SQL Server (LocalDB)
- MediatR (Mediator Pattern)
- ASP.NET Core Identity (Authentication)
- Serilog (Logging)
- SEQ (Log viewer at http://localhost:5341)
- Bootstrap 5

---

## Project Structure
DMS.sln
├── DMS               → MVC project (controllers, views, program.cs)
├── DMS.Data          → DbContext, EF configurations, migrations
├── DMS.Models        → Entities, contracts (requests/responses)
└── DMS.Handlers      → MediatR handlers, file storage service

---

## How to run

### Prerequisites
- Visual Studio 2022
- .NET 8 SDK
- SQL Server LocalDB
- SEQ (optional, for logs) — https://datalust.co/seq

### Steps
1. Clone the repo
2. Open `DMS.sln` in Visual Studio
3. Update connection string in `DMS/appsettings.json` if needed
4. Open Package Manager Console
5. Set default project to `DMS.Data`
6. Run migrations:

Add-Migration InitialCreate -StartupProject DMS
Update-Database -StartupProject DMS
7. Press F5 to run

---

## Default Admin Account
Email:    admin@dms.com
Password: Admin@123
This gets seeded automatically on first run.
Any new user that registers gets the User role by default.

---

## Roles and Permissions

| Action | Admin | User |
|---|---|---|
| Upload documents | Yes | Yes |
| View documents | All | Own only |
| Download documents | All | Own only |
| Edit documents | Any | Own only |
| Delete documents | Any | Own only |
| Add/remove tags | Any doc | Own docs |
| Create tags | Yes | No |
| View dashboard | Full stats | Own stats |

---

## Logging
Logs go to three places:
- Console (while running)
- `Logs/` folder as daily rolling files
- SEQ at http://localhost:5341 (if running)

Every HTTP request is logged automatically via Serilog request logging middleware.

---

## Notes
- Files are stored in `wwwroot/uploads/` folder
- Deleted documents are soft deleted (IsDeleted flag), not removed from DB
- Every document action is recorded in DocumentAuditLogs table
- Pagination is set to 10 items per page
- Dashboard shows top 10 recently uploaded and top 10 most accessed docs

---

## Known Issues / TODO
- No email confirmation on register
- No forgot password flow
- No file type restriction on upload (any file works)
- Pagination numbers could get long if there are too many pages

---

Made by Vansh | B.Tech CSE 2026