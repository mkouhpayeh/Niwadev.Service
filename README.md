# ⚡ Energy Service API

A sample .NET 9 Web API for managing customers, products, tariffs, and orders for energy-based services (electricity, gas, etc.).

---

## 🧩 Overview

This project demonstrates a typical business structure for energy contract management:

```
Customer
 └── Order
      └── OrderDetail
           ├── Product
           └── Tariff → TariffPrice → Unit
```

Each tariff can have multiple time-based prices (`TariffPrice`), 
and each price is associated with a measurement unit (kWh, m³, etc.).

Orders can use **fixed** or **floating** prices:
- **Fixed price** → stores a snapshot of the `TariffPrice` used at order time.
- **Floating price** → no snapshot; price is resolved dynamically during billing.

---

## 🧠 Tech Stack

- **.NET 9 / ASP.NET Core Web API**
- **Entity Framework Core**
- **SQL Server (default)** 
- **Swagger / OpenAPI** enabled by default

---

## 🗂️ Project Structure

| Folder | Description |
|--------|--------------|
| `Models/` | Entity classes (`Customer`, `Product`, `Tariff`, `TariffPrice`, `Unit`, `Order`, `OrderDetail`) |
| `Data/` | `AppDbContext` and `SeedData` for initialization |
| `Controllers/` | API endpoints (to be extended) |
| `Migrations/` | EF Core migrations |

---

## ⚙️ Configuration

Edit your **appsettings.json** to set the connection string:

```json
"ConnectionStrings": {
  "DefaultConnection": "xxx"
}
```

---


## 🌱 Seeding Data

On first startup, the app automatically seeds demo data.

To reseed the database (for development only), use:
```csharp
SeedData.Initialize(db, force: true);
```
This clears all tables and re-inserts demo data.

---

## 🧱 Migrations

Generate a new migration whenever you modify the model classes:

```bash
dotnet ef migrations add AddNewFieldName
dotnet ef database update
```

---

## 👤 Author

Created by **Mahi** © 2025
