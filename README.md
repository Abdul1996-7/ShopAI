# ShopAI

Phase 1 of a multi-tenant ASP.NET Core MVC e-commerce SaaS foundation. This phase includes store, catalog, dashboard, Identity, image upload, and seed data only.

## Setup

1. Update `ConnectionStrings:DefaultConnection` in `appsettings.json` for your SQL Server instance.
2. Create and apply the initial EF Core migration:

```powershell
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate
dotnet ef database update
```

3. Run the app:

```powershell
dotnet run
```

Development mode runs `DataSeeder` to create the demo categories, store, and sample products.

## NuGet Packages

- `Microsoft.AspNetCore.Authentication.JwtBearer` 8.0.0
- `Microsoft.AspNetCore.Identity.EntityFrameworkCore` 8.0.0
- `Microsoft.EntityFrameworkCore` 8.0.0
- `Microsoft.EntityFrameworkCore.Design` 8.0.0
- `Microsoft.EntityFrameworkCore.SqlServer` 8.0.0
- `SixLabors.ImageSharp` 3.1.11

## EF Migration Command

```powershell
dotnet ef migrations add InitialCreate
dotnet ef database update
