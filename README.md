# JwtAuthenticationDemo

A simple ASP.NET Core Web API project demonstrating JWT Authentication and Refresh Token implementation.

## 🔐 Features

- User login with JWT token generation
- Refresh token mechanism using `RefreshSessions` table
- Role-based authorization
- EF Core DB-first approach
- Auto-generated CRUD controllers
- IIS-compatible (ready to host locally or on a server)

## 🧱 Database Schema

- **User**: `Id`, `Username`, `Password`, `Email`, `RoleId`
- **UserRole**: `Id`, `RoleName`
- **RefreshSessions**: `SessionId`, `UserId`, `RefreshToken`, `ExpiryDate`, `CreatedAt`

## 🛠️ Tech Stack

- ASP.NET Core (.NET 8)
- Entity Framework Core
- SQL Server
- JWT Bearer Authentication
- Swagger for API testing

## 🔧 How to Run

1. Clone the repository:
   ```bash
   git clone https://github.com/Yuvaraj-Patil/JwtAuthenticationDemo.git
   cd JwtAuthenticationDemo
   dotnet build
   dotnet run'''
