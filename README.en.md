# FluvAuto - Auto Workshop Management System - [Versão PT aqui](README.md)

[![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-8.0-purple.svg)](https://dotnet.microsoft.com/apps/aspnet)
[![Entity Framework](https://img.shields.io/badge/Entity%20Framework-Core-orange.svg)](https://docs.microsoft.com/en-us/ef/)
[![Bootstrap](https://img.shields.io/badge/Bootstrap-5.x-purple.svg)](https://getbootstrap.com/)

## Description

**FluvAuto** is a web system for managing automotive workshops, built with **ASP.NET Core 8.0** and **Entity Framework Core**. The system allows you to manage customers, employees, vehicles, service appointments, and track the workshop workflow.

## Technologies Used

### **Backend**
- **ASP.NET Core 8.0** - Web framework
- **Entity Framework Core** - ORM for database access
- **ASP.NET Identity** - Authentication and authorization
- **SQL Server / SQLite** - Database
- **Swagger/OpenAPI** - API documentation

### **Frontend**
- **Razor Pages** - Templating engine
- **Bootstrap 5** - CSS framework
- **JavaScript** - Dynamic functionality
- **HTML5 & CSS3** - Structure and styling

## How to Run

### **Prerequisites**
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/sql-server) or SQLite
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)

### **Installation**

1. **Clone the repository:**
```bash
git clone https://github.com/Campi98/DWeb_FluvAuto.git
cd fluvAuto
```

2. **Configure the connection string:**
   - Edit `appsettings.json`
   - Configure the connection to SQL Server or SQLite

3. **Apply the migrations:**
```bash
Update-Database
```

5. **Run the application:**
```bash
dotnet run    (or click Run in VS2022)
```

6. **Access in the browser:**
   - **Web:** `https://localhost:7001`
   - **API:** `https://localhost:7001/swagger`

## Authentication

### **Default Users (created by DBInitializer):**

| Type | Email | Password | Permissions |
|------|-------|----------|-------------|
| Admin | admin@admin.com | Admin12345! | Full access |
| Employee | maria@email.com | Password123! | Service management |
| Customer | joao@email.com | Password123! | View own data |
