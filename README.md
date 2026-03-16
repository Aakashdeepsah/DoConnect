# ⚡ DoConnect — Technical Q&A Platform

> A full-stack Q&A platform where developers can ask questions, share knowledge, and grow together — built with **ASP.NET Core Web API** and **Angular**.

---

## 📌 What is DoConnect?

DoConnect is a moderated question-and-answer platform for technical topics. Users can post questions and answers with optional image attachments. An admin reviews and approves all content before it goes live, keeping the platform clean and reliable.

---

## 🛠 Tech Stack

| Layer | Technology |
|-------|-----------|
| Frontend | Angular 16 |
| Backend | ASP.NET Core 8 Web API (MVC pattern) |
| Database | SQL Server (via SSMS) |
| ORM | Entity Framework Core 8 |
| Authentication | JWT Bearer Tokens |
| Password Hashing | BCrypt |
| File Storage | Server-side `wwwroot/uploads` folder |
| API Documentation | Swagger UI |
| Unit Testing | xUnit + Moq + EF InMemory |

---

## 👤 User Roles

### Regular User
- Register and log in
- Ask questions under technical topics (Angular, .NET, SQL, JavaScript, etc.)
- Answer approved questions
- Attach images to questions and answers
- Search and browse all approved content

### Admin
- Review and approve or reject pending questions and answers
- Delete inappropriate content
- View a live notification badge showing items pending review
- Access all content regardless of status (Pending, Approved, Rejected)

> ⚠️ **Admin accounts cannot be created through the registration form.**
> They are created directly in the database by the system administrator.
> See [Setup → Creating an Admin](#-creating-an-admin-account) below.

---

## ✅ Key Features

- **Moderated content** — all questions and answers require admin approval before going public
- **Image uploads** — attach screenshots or diagrams to questions and answers (JPG, PNG, GIF, WebP — max 5MB)
- **Full-text search** — search across question titles, topics, and body text
- **JWT authentication** — secure sessions with auto-expiry
- **Role-based access** — User and Admin roles enforced on both frontend and backend
- **Pending count badge** — admins see a live count of items waiting for review in the navbar
- **Professional dark UI** — responsive Angular frontend with a dark theme

---

## 🗂 Project Structure

```
DoConnect/
│
├── DoConnect.sln                  ← Open this in Visual Studio
├── DoConnect.code-workspace       ← Open this in VS Code (Angular)
│
├── Database/
│   └── doconnect_schema.sql       ← Run this in SSMS first
│
├── Backend/
│   └── DoConnect.API/
│       ├── Controllers/           ← AuthController, QuestionController, AnswerController, AdminController
│       ├── Models/                ← User, Question, Answer
│       ├── DTOs/                  ← Data Transfer Objects (request/response shapes)
│       ├── Services/              ← Business logic
│       ├── Interfaces/            ← Service contracts
│       ├── Data/                  ← EF Core DbContext
│       ├── Helpers/               ← JWT helper, Image helper
│       ├── Middleware/            ← Global exception handler
│       ├── wwwroot/uploads/       ← Uploaded images stored here
│       ├── appsettings.json       ← DB connection string + JWT config
│       └── Program.cs             ← App startup
│
├── Tests/
│   └── DoConnect.Tests/           ← xUnit unit tests (21 tests)
│
└── Frontend/
    └── doconnect-frontend/        ← Angular app
        └── src/app/
            ├── components/        ← landing, navbar, login, register, home, ask-question, question-detail, admin
            ├── services/          ← auth, question, answer, admin services
            ├── guards/            ← AuthGuard, AdminGuard
            ├── interceptors/      ← JWT interceptor (attaches token to every request)
            └── models/            ← TypeScript interfaces
```

---

## 🚀 Getting Started

### Prerequisites

Make sure you have these installed:

- [Visual Studio 2022](https://visualstudio.microsoft.com/) (with ASP.NET workload)
- [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) + [SSMS](https://learn.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms)
- [Node.js LTS](https://nodejs.org/) (v18 or v20)
- [Angular CLI](https://angular.io/cli) — install with `npm install -g @angular/cli`
- [VS Code](https://code.visualstudio.com/)

---

### Step 1 — Database Setup (SSMS)

1. Open **SQL Server Management Studio**
2. Click **File → Open → File** and open `Database/doconnect_schema.sql`
3. Press **F5** to run it
4. The `DoConnectDB` database is created with all tables

> The script is **idempotent** — safe to run multiple times without errors.

---

### Step 2 — Backend Setup (Visual Studio)

1. Open **Visual Studio 2022**
2. Click **Open a project or solution** → select `DoConnect.sln`
3. Open `Backend/DoConnect.API/appsettings.json`
4. Update the connection string to match **your** SQL Server name:

```json
"DefaultConnection": "Server=.\\SQLEXPRESS;Database=DoConnectDB;Trusted_Connection=True;TrustServerCertificate=True;"
```

> Common server names: `.\SQLEXPRESS`, `localhost`, `DESKTOP-XXXXX\SQLEXPRESS`
> Check the top of SSMS — it shows your server name when you connect.

5. In Solution Explorer, right-click **DoConnect.API** → **Set as Startup Project**
6. Press **F5** — Swagger opens at `https://localhost:7001/swagger`

---

### Step 3 — Frontend Setup (VS Code)

1. Double-click **`DoConnect.code-workspace`** — VS Code opens the Angular project
2. Open the terminal in VS Code (`Ctrl + ~`)
3. Run:

```bash
npm install
```

4. Once installed, start the app:

```bash
npm start
```

5. Open your browser at **http://localhost:4200**

> The Angular app automatically proxies all `/api` requests to the backend at `https://localhost:7001`.
> If your backend runs on a different port, update `proxy.conf.json`.

---

### Step 4 — Creating an Admin Account

The registration page only creates **User** accounts. To create an Admin:

1. Register normally through the app at `/register`
2. Open **SSMS** and run:

```sql
USE DoConnectDB;

UPDATE Users
SET Role = 'Admin'
WHERE Email = 'your-email@example.com';
```

3. Log out of the app and log back in — you will be redirected to the Admin Dashboard

---

## 🔑 Default Credentials (from seed data)

| Account | Email | Password |
|---------|-------|----------|
| Admin | admin@doconnect.com | Admin@123 |

> You can also register your own account and promote it via SSMS as shown above.

---

## 🧪 Running Unit Tests

### In Visual Studio
Go to **Test → Test Explorer** → click **Run All Tests**

### In the terminal
```bash
cd Tests/DoConnect.Tests
dotnet test
```

Expected result: **All tests pass** ✅

### What is tested
- Auth: registration always creates User role, duplicate email/username rejected, login success/failure
- Questions: approved-only filter, GetById hides pending questions, search, create sets Pending status
- Answers: cannot answer a pending/rejected question, answer creates with Pending status
- Admin: approve/reject questions and answers, cascade delete, pending count

---

## 🌐 API Endpoints

Full documentation available at `https://localhost:7001/swagger` when the backend is running.

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/api/auth/register` | None | Register new user |
| POST | `/api/auth/login` | None | Login, returns JWT |
| GET | `/api/question` | None | All approved questions |
| GET | `/api/question/search?query=` | None | Search questions |
| GET | `/api/question/{id}` | None | Single approved question |
| POST | `/api/question` | User | Ask a question |
| GET | `/api/answer/{questionId}` | None | Approved answers |
| POST | `/api/answer` | User | Post an answer |
| GET | `/api/admin/questions` | Admin | All questions (any status) |
| PUT | `/api/admin/questions/{id}/status` | Admin | Approve or reject |
| DELETE | `/api/admin/questions/{id}` | Admin | Delete question |
| GET | `/api/admin/answers` | Admin | All answers |
| PUT | `/api/admin/answers/{id}/status` | Admin | Approve or reject |
| DELETE | `/api/admin/answers/{id}` | Admin | Delete answer |
| GET | `/api/admin/pending-count` | Admin | Count of pending items |

### How to test protected routes in Swagger
1. Call `/api/auth/login` and copy the `token` from the response
2. Click the **Authorize** 🔒 button at the top of Swagger
3. Enter: `Bearer your_token_here`
4. Click **Authorize** — all protected endpoints now work

---

## ⚙️ Configuration

### Changing the backend port
Edit `Backend/DoConnect.API/Properties/launchSettings.json`:
```json
"applicationUrl": "https://localhost:7001"
```
Then update `Frontend/doconnect-frontend/proxy.conf.json` to match:
```json
{ "/api": { "target": "https://localhost:7001" } }
```

### JWT Settings (`appsettings.json`)
```json
"JwtSettings": {
  "Secret": "your-secret-key-minimum-32-characters",
  "Issuer": "DoConnect.API",
  "Audience": "DoConnect.Client",
  "ExpiresInHours": 24
}
```

---

## 🐛 Common Issues

| Problem | Fix |
|---------|-----|
| `Cannot connect to database` | Check `appsettings.json` — update the `Server=` value to match your SQL Server name |
| `SQL Server service not running` | Press Win+R → `services.msc` → find `SQL Server (SQLEXPRESS)` → Start |
| `CORS error in browser` | Make sure backend is running and `proxy.conf.json` port matches |
| `npm install fails (ECONNRESET)` | Run `npm config set registry https://registry.npmmirror.com` then retry |
| `Images not showing` | Make sure `wwwroot/uploads/` folder exists in the API project and `app.UseStaticFiles()` is in `Program.cs` |
| `401 Unauthorized` | Make sure you are logged in as an account with the correct role |

---

## 📁 Development Workflow

Every time you work on this project:

1. **Start backend:** Open `DoConnect.sln` in Visual Studio → Press F5
2. **Start frontend:** Open `DoConnect.code-workspace` in VS Code → Terminal → `npm start`
3. **Open browser:** `http://localhost:4200`

Both must be running at the same time.

---

## 👨‍💻 Author

**Aakash Deep Sah**
Wipro Pre-Skilling Capstone Project — Full Stack .NET + Angular

---

## 📄 License

This project was built as a capstone submission for educational purposes.
