# 📘 Student Study Portal

## 📌 Overview
The **Student Study Portal** is a web-based platform built using **ASP.NET Core MVC**, **Entity Framework Core**, and **SQL Server**.  
It provides students with study management tools like Homework, To-Do lists, Books, YouTube integration, Wikipedia access.  
It also has an Admin role for viewing student details and audit logs.

---

## 🚀 Features
- 🔑 **Authentication**: Register, Login, Logout with session & cookie authentication.
- 📝 **Homework Management**: Add, view, delete, update and manage homework. Stored in both `Homework` and `Store` tables.
- ✅ **To-Do List**: Add, complete, update and delete tasks.
- 📚 **Books**: Access study-related books (static/dynamic).
- 🎥 **YouTube Integration**: Search and display personalized videos using API Key.
- 🌍 **Wikipedia Search**: Embedded search for quick learning.
- 📊 **Audit Log**: Tracks login and logout times for students and admin.
- 🔐 **Forgot Password**: Password reset using Email + Token system.
- 👨‍🎓 **Roles**:
  - **Student** → Homework, To-Do, YouTube, Wikipedia, Books, Records.
  - **Admin** → Can only view Student records & Audit logs.

---

## 🛠️ Project Structure
```
Student-study-portal/
│── SSP.sln                   # Solution file
│── SSP/                      # Main ASP.NET Core Project
│   ├── Controllers/          # MVC Controllers
│   ├── Models/               # Domain & View Models
│   ├── Data/                 # DbContext & EF Core Configurations
│   ├── Migrations/           # EF Core Migrations
│   ├── Views/                # Razor Views (UI)
│   ├── wwwroot/              # Static files (CSS, JS, Images)
│   ├── appsettings.json      # Configuration (Database, API Keys, Email)
│   └── Program.cs            # Application entry point
```

---

## ⚙️ Installation & Setup

### 1️⃣ Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) with ASP.NET workload
- [SQL Server Management Studio (SSMS)](https://aka.ms/ssmsfullsetup)

### 2️⃣ Clone Repository
```bash
git clone https://github.com/your-repo/student-study-portal.git
cd student-study-portal/SSP
```

### 3️⃣ Database Setup
1. Open **SSMS** and create a new database:
```sql
CREATE DATABASE StudyPortalDB;
```
2. Update **connection string** in `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=StudyPortalDB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```
3. Apply migrations:
```bash
Enable-Migrations
Add-Migration InitialCreate
Update-Database
```

### 4️⃣ Required Packages
Installed via NuGet in Visual Studio:
- `Microsoft.EntityFrameworkCore.SqlServer`
- `Microsoft.EntityFrameworkCore.Tools`
- `Microsoft.AspNetCore.Authentication.Cookies`
- `Microsoft.Extensions.Configuration.UserSecrets`
- `MailKit` (for email sending in Forgot Password)
- `Newtonsoft.Json`
-`Microsoft.VisualStudio.Web.CodeGeneration.Design`
- `Microsoft.AspNetCore.Identity.UI`

### 5️⃣ API Keys & Secrets
Add your You Tube keys in **`YouTube.cshtml`**:

```
  "ApiKey": "YOUR_YOUTUBE_API_KEY"
```

Add your Forgot Password API  keys in **`appsetting.json`**:

```
 "Email": {
   "Username": "xyz@gmail.com",
   "Password": "YOUR_EMAIL_APP_PASSWORD"
 }
```

---

## ▶️ Running the Project
1. Open solution in Visual Studio (`SSP.sln`).
2. Set **SSP** as Startup Project.
3. Run the project (IIS Express / Kestrel).
4. Browser will open → Home Page → Register / Login.

---

## 📊 Default Roles
- **Admin**: Auto-created in `Program.cs` when database is seeded.
- **Student**: Registers manually via Register form.

---

## 🔑 Forgot Password Flow
1. User enters email in Forgot Password form.
2. System generates reset token (stored in DB).
3. Email with reset link is sent using SMTP (MailKit).
4. User clicks link → Reset Password form → New password saved.

---


## 👨‍💻 Author
Developed by **[Jadav Yoganshi]**  
B.Tech Agriculture IT | ASP.NET Core Developer

---

## 📥 Download Project
You can download the project as a **ZIP** from here:  
👉 [Student Study Portal](./student-study-portal.zip)
