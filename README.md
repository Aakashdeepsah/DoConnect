# ⚡ DoConnect

A moderated Q&A platform for technical topics, built as a full-stack capstone project using **ASP.NET Core Web API** and **Angular 16**.

---

## Overview

DoConnect lets users ask and answer technical questions across topics like Angular, .NET, SQL, and JavaScript. All content goes through admin moderation before it's visible publicly — keeping the platform clean and reliable. Users can attach images to both questions and answers.

---

## Tech Stack

- **Frontend** — Angular 16, TypeScript, RxJS
- **Backend** — ASP.NET Core 8 Web API (MVC pattern)
- **Database** — SQL Server with Entity Framework Core 8
- **Auth** — JWT Bearer tokens, BCrypt password hashing
- **Testing** — xUnit, Moq, EF InMemory
- **Docs** — Swagger UI

---

## Features

- Ask and answer questions with optional image attachments
- Full-text search across titles, topics, and question bodies
- Admin approval workflow — nothing goes live without review
- Role-based access (User / Admin) enforced on both frontend and backend
- Live pending-item badge in the admin navbar
- JWT authentication with auto-expiry
- Global exception handling middleware
- 21 unit tests covering auth, questions, answers, and admin operations

---

## Project Structure

```
DoConnect/
├── Backend/DoConnect.API/
│   ├── Controllers/        # Auth, Question, Answer, Admin
│   ├── Models/             # User, Question, Answer
│   ├── Services/           # Business logic layer
│   ├── DTOs/               # Request / response shapes
│   ├── Data/               # EF Core DbContext
│   ├── Helpers/            # JWT, Image upload
│   ├── Middleware/         # Global exception handler
│   └── wwwroot/uploads/    # Stored image files
├── Tests/DoConnect.Tests/  # xUnit unit tests
├── Frontend/doconnect-frontend/  # Angular app
└── Database/               # SQL schema script
```

---

## Getting Started

### Prerequisites

- Visual Studio 2022
- SQL Server Express + SSMS
- Node.js (v18+) and Angular CLI (`npm install -g @angular/cli`)
- VS Code

### Database

Run `Database/doconnect_schema.sql` in SSMS. The script is idempotent — safe to run multiple times.

### Backend

Open `DoConnect.sln` in Visual Studio. Update the connection string in `appsettings.json` to match your SQL Server instance, then press **F5**. Swagger is available at `https://localhost:7001/swagger`.

```json
"DefaultConnection": "Server=.\\SQLEXPRESS;Database=DoConnectDB;Trusted_Connection=True;TrustServerCertificate=True;"
```

### Frontend

```bash
cd Frontend/doconnect-frontend
npm install
npm start
```

Open `http://localhost:4200`. The Angular proxy forwards all `/api` requests to the backend automatically.

---

## Admin Access

The registration form only creates User accounts by design. To grant admin access, update the role directly in the database after registering:

```sql
UPDATE Users SET Role = 'Admin' WHERE Email = 'your@email.com';
```

Then log out and back in.

---

## API Reference

Full interactive docs at `/swagger` when the backend is running.

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/api/auth/register` | — | Register |
| POST | `/api/auth/login` | — | Login, returns JWT |
| GET | `/api/question` | — | All approved questions |
| GET | `/api/question/search?query=` | — | Search |
| GET | `/api/question/{id}` | — | Single question |
| POST | `/api/question` | User | Ask a question |
| GET | `/api/answer/{questionId}` | — | Answers for a question |
| POST | `/api/answer` | User | Post an answer |
| GET | `/api/admin/questions` | Admin | All questions |
| PUT | `/api/admin/questions/{id}/status` | Admin | Approve / Reject |
| DELETE | `/api/admin/questions/{id}` | Admin | Delete |
| GET | `/api/admin/answers` | Admin | All answers |
| PUT | `/api/admin/answers/{id}/status` | Admin | Approve / Reject |
| DELETE | `/api/admin/answers/{id}` | Admin | Delete |
| GET | `/api/admin/pending-count` | Admin | Pending items count |

---

## Running Tests

```bash
cd Tests/DoConnect.Tests
dotnet test
```

Or use **Test Explorer** in Visual Studio.

---

## Notes

- Images are stored in `wwwroot/uploads/` and served as static files
- The proxy target in `proxy.conf.json` must match the backend port (default `7001`)
- Admin accounts are seeded via SQL — they cannot be self-registered

---

**Aakash Deep Sah** — Wipro Pre-Skilling Capstone
