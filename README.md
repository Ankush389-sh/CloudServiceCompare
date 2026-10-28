# CloudCompare - Cloud Service Provider Comparison (ASP.NET Core MVC + EF Core + MySQL)

Web Technologies with .NET - Mini Project

## Problem statement
Choosing between cloud providers (AWS, Azure, Google Cloud) is confusing. CloudCompare lets users browse
providers and services, compare price / uptime SLA / regions side by side, and read or write user reviews.

## Features
- MVC architecture (Models / Views / Controllers)
- EF Core + MySQL (Pomelo provider) on XAMPP
- CRUD: Providers, Services (admin); Reviews (create by anyone, delete by admin)
- Search, filter by category/provider, sort by price/SLA/rating
- Compare up to 3 services of the same category (best value highlighted)
- Provider ranking and price chart (Chart.js) on the home page
- Form handling + model validation (server + client side, duplicate checks)
- State management: Session (admin login + compare list), TempData (messages), Cookie (dark/light theme)
- Responsive UI (Bootstrap 5)

## Setup with XAMPP
1. Install .NET 8 SDK. Open XAMPP -> Start **MySQL**.
2. Check `appsettings.json`:
   Server=localhost;Port=3306;Database=cloud_compare_db;User=root;Password=;
3. In this folder run:
   dotnet restore
   dotnet run
4. Open the URL shown in the terminal. Admin login (top right): admin / admin123

The database, tables and sample data are created automatically on first run.
Sample prices, SLAs and regions are illustrative - not official figures.

## Structure
Controllers/ Account, Home, Providers, Services, Reviews, Compare
Models/      Provider, CloudService, Review, view models
Data/        AppDbContext, DbSeeder
Filters/     AdminAuthAttribute
Views/       Razor views

## UI customisation
- Colours: edit the variables at the top of `wwwroot/css/site.css` (`--brand`, `--brand-dark`, `--brand-rgb`, `--brand-2`),
  or use the 4 coloured dots in the navbar (Indigo / Ocean / Emerald / Sunset). The choice is saved in a cookie.
- Smooth behaviour lives in `wwwroot/js/site.js`: page fade, live filtering, AJAX compare button, theme toggle without reload.
