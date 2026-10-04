# MediCore — C# ASP.NET Core Project

## Project Structure

```
MediCore/
├── Controllers/
│   ├── AuthController.cs       ← Login / logout / session
│   ├── PatientsController.cs   ← Patient CRUD + next-ID
│   └── RecordsController.cs    ← Visit records
├── Data/
│   └── MediCoreDb.cs           ← In-memory database + seed data
│                                  ⚠️  CREDENTIALS LIVE HERE — keep private!
├── Models/
│   └── Models.cs               ← All C# model classes
├── wwwroot/
│   └── index.html              ← Full frontend (HTML + CSS + JS)
├── Program.cs                  ← ASP.NET Core startup
└── MediCore.csproj
```

---

## Prerequisites

Install **.NET 8 SDK** (free):
👉 https://dotnet.microsoft.com/download/dotnet/8.0

Verify installation:
```bash
dotnet --version
# Should print: 8.0.x
```

---

## How to Run

### 1. Open a terminal and navigate to the project folder:
```bash
cd path/to/MediCore
```

### 2. Restore packages & run:
```bash
dotnet run
```

### 3. Open your browser:
```
http://localhost:5000
```
(or whatever port is shown in the terminal — e.g. `http://localhost:5136`)

---

## Login Credentials

Credentials are stored **only** in `Data/MediCoreDb.cs` on the server.
They are **never** sent to or displayed in the browser.

| Role    | Username  | Password  |
|---------|-----------|-----------|
| Admin   | admin     | admin123  |
| Doctor  | doctor    | doc123    |
| Doctor  | doctor2   | doc456    |

**Patients** log in using their Patient ID (e.g. `MC-2026-001`).

### Change passwords before deploying:
Open `Data/MediCoreDb.cs` and update the `_staff` dictionary:
```csharp
["admin"]   = ("YOUR_NEW_PASSWORD",  "admin",  "Admin User",       "Administration"),
["doctor"]  = ("YOUR_NEW_PASSWORD",  "doctor", "Dr. Imran Khan",   "General Physician"),
["doctor2"] = ("YOUR_NEW_PASSWORD",  "doctor", "Dr. Fareeha Shah", "Internal Medicine"),
```

---

## API Endpoints

| Method | URL                            | Access        | Description                  |
|--------|-------------------------------|---------------|------------------------------|
| POST   | /api/auth/login                | Public        | Staff login                  |
| POST   | /api/auth/patient-login        | Public        | Patient login by ID          |
| POST   | /api/auth/logout               | Authenticated | Log out                      |
| GET    | /api/auth/me                   | Authenticated | Get current session          |
| GET    | /api/patients                  | Staff only    | List all patients            |
| GET    | /api/patients/{id}             | Staff + self  | Get patient by ID            |
| GET    | /api/patients/{id}/records     | Staff + self  | Get visit records            |
| POST   | /api/patients                  | Admin only    | Register new patient         |
| GET    | /api/patients/next-id          | Admin only    | Preview next patient ID      |
| GET    | /api/records                   | Staff only    | All visit records            |
| POST   | /api/records                   | Staff only    | Add new visit record         |

---

## Data Storage

By default the app uses **in-memory storage** — data resets when the server restarts.

### To persist data with a real database:

1. Add EF Core package:
```bash
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
```

2. Replace the `MediCoreDb` singleton with an EF Core `DbContext`.
   All the models in `Models/Models.cs` are already compatible.

---

## Publishing / Deploying

Build a self-contained release:
```bash
dotnet publish -c Release -o ./publish
```

Run the published version:
```bash
./publish/MediCore
```

For production, put it behind **nginx** or **IIS** as a reverse proxy.

---

## Notes

- The login form shows **no demo credentials** — this is intentional.
- Session cookies are `HttpOnly` and `SameSite=Strict`.
- All role-based access is enforced on the **server side** in each controller.
- The frontend communicates only through the REST API — no credentials ever leave the server.
