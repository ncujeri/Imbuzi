# iMbuzi Smart - Livestock Manager PWA
iMbuzi Smart is an offline-first, mobile-first livestock management Progressive Web App (PWA) specifically designed for rural South African goat farmers. It bridges the gap between traditional farming and data-driven agriculture by solving for limited internet connectivity and the lack of physical weighing scales.
## 🚀 Vision
 * **Virtual Scale:** Estimate weight using Heart Girth formulas—no scale required.
 * **Offline-First:** Full functionality in deep rural areas with zero signal.
 * **Trust-Based Selling:** Generate "Digital Passports" to prove animal health and growth to buyers.
 * **Culturally Relevant:** High-contrast UI optimized for outdoor use, with support for local context.
## 🏗️ Project Structure
The solution is built on a **Blazor WebAssembly Hosted** architecture using **.NET 10**.
```text
/home/user/Imbuzi/
├── src/
│   ├── ImbuziSmart.Server/        # ASP.NET Core API & WASM Host
│   │   ├── Data/                  # EF Core + Global Tenant Filters
│   │   └── Controllers/           # Sync & Batch API Endpoints
│   │
│   ├── ImbuziSmart.Client/        # Blazor WASM PWA (The User Interface)
│   │   ├── Components/            # Fluent UI Dashboards & Touch-Ready Buttons
│   │   ├── Services/              # Weight, Gestation, & Heat Logic
│   │   └── wwwroot/               # PWA Manifest, Service Workers, & IndexedDB
│   │
│   └── ImbuziSmart.Shared/        # Models & Enums shared across projects
│       ├── Entities/              # Animal, Mating, Medical, & Cost Models
│       └── ValueObjects/          # Weight & Photo records
│
└── tests/
    └── ImbuziSmart.Tests/         # XUnit tests for core agricultural logic

```
## 🛠️ Implementation Phases
### Phase 1: Foundation & Scaffolding
 * **SDK:** Target net10.0 in Directory.Build.props.
 * **UI Framework:** Integrated **Microsoft Fluent UI Blazor** for a native mobile feel.
 * **PWA:** Service workers configured for asset caching and offline resilience.
### Phase 2: Domain Logic (The "Goat Rules")
 * **Gestation:** Automated 150-day countdown with "Kidding Watch" alerts.
 * **Heat Tracking:** Predicts 21-day estrus cycles to optimize breeding.
 * **Inbreeding Check:** Prevents sire/dam collisions during mating logs.
 * **Withdrawal Tracker:** Blocks "Market Ready" status if medication withdrawal periods are active.
### Phase 3: The Virtual Scale
The application utilizes the **Schaeffer Formula** to calculate weight without a physical scale:

### Phase 4: Offline Data Engine
 * **Local Storage:** Uses **IndexedDB** via JS Interop for all livestock records.
 * **Sync Queue:** Logs changes locally and pushes to the SQL Server backend via SyncController once a network connection is detected.
 * **Multi-Tenancy:** Global query filters ensure farmers only see their own herd data.
## 📱 User Interface (Mobile-First)
### QuickLog Dashboard
The home screen features a high-contrast grid of large touch targets:
 * 🟢 **New Animal:** Log births or purchases.
 * 🔴 **Medical Log:** Record vaccinations/dips (Triggers timers).
 * 🔵 **Weigh Animal:** Input girth measurements for the Virtual Scale.
 * 🟣 **Record Mating:** Start the gestation clock.
 * 💗 **Log Heat:** Track "Ready for Buck" cycles.
 * 🟠 **Log Cost:** Sync feed/medicine expenses to the **Shadow Ledger**.
### Settings & Configuration
Farmers can toggle features to match their management style:
 * Enable/Disable Inbreeding warnings.
 * Customizable Notification triggers (Push API).
 * Configurable Heat Cycle lengths (Default 21 days).
## 🧪 Verification & Testing
```bash
# Build the solution
dotnet build ImbuziSmart.sln

# Run Agricultural Logic Tests (Weight/Gestation)
dotnet test

# Launch the Application
dotnet run --project src/Server

```
## 🛡️ Technical Stack
 * **Frontend:** Blazor WASM, Fluent UI, PWA.
 * **Backend:** ASP.NET Core, EF Core, SQL Server.
 * **Security:** IdentityServer4 / OpenID Connect.
 * **Storage:** IndexedDB (Client), Blob Storage (Photos).
