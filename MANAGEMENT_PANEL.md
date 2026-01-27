# Gaming Center Management Panel - Implementation Guide

## Overview
This implementation adds a comprehensive management panel to the Gck (Gaming Center Kowthar) project. The management panel allows administrators to manage tables, customers, sessions, and financial transactions.

## Architecture

The solution follows a clean architecture pattern with the following layers:

```
Gck.sln
├── Gck.Domain          # Entity models
├── Gck.Persistence     # Database context and migrations
├── Gck.Application     # CQRS commands/queries (MediatR)
├── Gck.Api             # ASP.NET Core Web API
└── Gck                 # Blazor WebAssembly UI
```

## Database Entities

### 1. Table
Represents gaming stations with TV, PS5, and controllers.
- **Fields**: Id, Name, NumberOfControllers, HourlyFeePerController, IsOccupied
- **Location**: `Gck.Domain/Entities/Table.cs`

### 2. Customer
Stores customer information.
- **Fields**: Id, Name, PhoneNumber, BirthYear, Gender (default: "Male")
- **Location**: `Gck.Domain/Entities/Customer.cs`

### 3. FinancialAccount
Tracks financial accounts for payments.
- **Fields**: Id, AccountName, CardNumber, BankName
- **Location**: `Gck.Domain/Entities/FinancialAccount.cs`

### 4. Session
Manages gaming sessions at tables.
- **Fields**: Id, TableId, FeePerHour, StartDateTime, EndDateTime, IsCompleted, RecommendedPrice, FinalPrice
- **Relationships**: 
  - Belongs to one Table
  - Can have multiple SessionCustomers
  - Has one AccountantReceipt when completed
- **Location**: `Gck.Domain/Entities/Session.cs`

### 5. SessionCustomer (Join Table)
Links customers to sessions (many-to-many).
- **Fields**: Id, SessionId, CustomerId
- **Location**: `Gck.Domain/Entities/SessionCustomer.cs`

### 6. AccountantReceipt
Financial receipt for completed sessions.
- **Fields**: Id, SessionId, FinancialAccountId, RecommendedPrice, FinalPrice, ReceiptDateTime
- **Location**: `Gck.Domain/Entities/AccountantReceipt.cs`

## Database Migration

A migration has been created: `20260106111339_AddGamingCenterManagement`

### Applying the Migration

To apply the migration to your database:

```bash
cd Gck.Api
dotnet ef database update
```

Or run the API project, and the migration will be automatically applied during startup.

## API Endpoints

### Tables API (`/api/tables`)
- `GET /api/tables` - Get all tables
- `GET /api/tables/{id}` - Get table by ID
- `POST /api/tables` - Create new table
- `PUT /api/tables/{id}` - Update table
- `DELETE /api/tables/{id}` - Delete table

### Customers API (`/api/customers`)
- `GET /api/customers` - Get all customers
- `POST /api/customers` - Create new customer

### Financial Accounts API (`/api/financialaccounts`)
- `GET /api/financialaccounts` - Get all financial accounts
- `POST /api/financialaccounts` - Create new financial account

### Sessions API (`/api/sessions`)
- `GET /api/sessions/{id}` - Get session by ID
- `POST /api/sessions/start` - Start new session
- `POST /api/sessions/{id}/finish` - Finish session
- `POST /api/sessions/{id}/resume` - Resume session

### Dashboard API (`/api/dashboard`)
- `GET /api/dashboard/tables` - Get dashboard tables with current sessions

## Running the Application

### 1. Run the API (Backend)

```bash
cd Gck.Api
dotnet run
```

The API will start on:
- HTTP: `http://localhost:5001`
- HTTPS: `https://localhost:7001`

Swagger UI is available at: `http://localhost:5001/swagger`

### 2. Run the Blazor App (Frontend)

In a separate terminal:

```bash
cd Gck
dotnet run
```

The Blazor app will start on:
- HTTP: `http://localhost:5193`
- HTTPS: `https://localhost:7193`

### 3. Access the Management Panel

1. Open your browser and navigate to the Blazor app URL
2. Login with a user account (or create one if needed)
3. Click on "مدیریت" (Management) in the navigation menu
4. The management panel will display tables, customers, and financial accounts

## Session Management Flow

### Starting a Session

1. **API Call**: `POST /api/sessions/start`
   ```json
   {
     "tableId": 1,
     "feePerHour": 100000,
     "customerIds": [1, 2]
   }
   ```

2. **Process**:
   - Validates table exists and is not occupied
   - Creates new session record
   - Marks table as occupied
   - Links customers to session (if provided)

3. **Returns**: Session ID

### Finishing a Session

1. **API Call**: `POST /api/sessions/{id}/finish`
   ```json
   {
     "sessionId": 1,
     "finalPrice": 150000,
     "financialAccountId": 1
   }
   ```

2. **Process**:
   - Calculates recommended price: `(EndTime - StartTime) × FeePerHour`
   - Records both recommended and final prices
   - Creates accountant receipt
   - Marks table as free
   - Marks session as completed

3. **Returns**: 204 No Content

### Resuming a Session

1. **API Call**: `POST /api/sessions/{id}/resume`

2. **Process**:
   - Clears end time and pricing from session
   - Allows session to continue

3. **Returns**: 204 No Content

## Key Features Implemented

### ✅ Completed

1. **Database Layer**
   - All 6 entity models created
   - Proper relationships and foreign keys
   - Database migration created
   - DbContext configured

2. **Application Layer**
   - CQRS pattern with MediatR
   - Commands for Create, Update, Delete operations
   - Queries for retrieving data
   - Session management (Start, Finish, Resume)

3. **API Layer**
   - RESTful controllers for all entities
   - Proper error handling
   - Swagger documentation

4. **UI Layer**
   - Basic management dashboard page
   - Display lists of tables, customers, and accounts
   - Navigation integration
   - Responsive design

### ⏳ Pending/Future Enhancements

1. **Persian Calendar Integration**
   - Currently using DateTime.UtcNow
   - Need to add Persian/Jalali calendar library
   - Update UI to display dates in Persian format

2. **Advanced Session Management UI**
   - Modal dialogs for starting sessions
   - Modal for finishing with price calculation
   - Real-time session timer display
   - Customer selection interface

3. **Dashboard Analytics**
   - Bar charts for daily receipts (this week)
   - Bar charts for monthly receipts (this year)
   - Persian date filtering

4. **Full CRUD UI**
   - Forms for creating/editing tables
   - Forms for creating/editing customers
   - Forms for creating/editing financial accounts
   - Delete confirmations

## Testing the Implementation

### 1. Test Database Setup

Run the API to automatically apply migrations:

```bash
cd Gck.Api
dotnet run
```

### 2. Test API Endpoints with Swagger

1. Navigate to `http://localhost:5001/swagger`
2. Try the following sequence:

   a. **Create a Table**:
   ```json
   POST /api/tables
   {
     "name": "میز 1",
     "numberOfControllers": 2,
     "hourlyFeePerController": 70000
   }
   ```

   b. **Create a Customer**:
   ```json
   POST /api/customers
   {
     "name": "علی احمدی",
     "phoneNumber": "09123456789",
     "birthYear": 1380,
     "gender": "Male"
   }
   ```

   c. **Create a Financial Account**:
   ```json
   POST /api/financialaccounts
   {
     "accountName": "حساب اصلی",
     "cardNumber": "6037-9977-1234-5678",
     "bankName": "بانک ملی"
   }
   ```

   d. **Start a Session**:
   ```json
   POST /api/sessions/start
   {
     "tableId": 1,
     "feePerHour": 140000,
     "customerIds": [1]
   }
   ```

   e. **Finish the Session**:
   ```json
   POST /api/sessions/1/finish
   {
     "sessionId": 1,
     "finalPrice": 150000,
     "financialAccountId": 1
   }
   ```

### 3. Test Blazor UI

1. Run both API and Blazor app
2. Navigate to `/management` page
3. Verify you can see:
   - Number of tables
   - Number of customers
   - Number of financial accounts
   - Number of occupied tables
4. Click on each management button to see the lists

## Configuration

### API Configuration

Edit `Gck.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=GckDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Change the connection string according to your SQL Server setup.

### CORS Configuration

The API is configured to allow requests from:
- `https://localhost:5001`
- `http://localhost:5000`
- `https://localhost:7001`
- `http://localhost:5193`
- `https://localhost:7193`

To add more origins, edit `Gck.Api/Program.cs`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient",
        policy => policy
            .WithOrigins(
                "https://localhost:5001",
                // Add more origins here
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});
```

## Dependencies

### API Project
- Microsoft.EntityFrameworkCore (9.0.0)
- Microsoft.EntityFrameworkCore.SqlServer (9.0.0)
- Microsoft.EntityFrameworkCore.Design (9.0.0)
- MediatR (12.4.1)
- Swashbuckle.AspNetCore (7.2.0)

### Application Project
- MediatR (12.4.1)
- AutoMapper (13.0.1)
- FluentValidation (11.9.2)

### Blazor Project
- Microsoft.AspNetCore.Components.WebAssembly (9.0.9)
- System.Net.Http.Json (10.0.1)
- Blazored.LocalStorage (4.5.0)
- BlazorPro.BlazorSize (8.0.0)

## Troubleshooting

### Issue: "Could not connect to API"

**Solution**: Make sure the API is running on `http://localhost:5001`. The Blazor app is configured to call this URL.

### Issue: "Database does not exist"

**Solution**: Run the API project once. It will automatically create the database and apply migrations on startup.

### Issue: "Table already exists" error

**Solution**: This means the migration has already been applied. You can safely ignore this or remove the migration and recreate it.

### Issue: "CORS error"

**Solution**: Ensure the Blazor app URL is added to the CORS policy in `Gck.Api/Program.cs`.

## Future Recommendations

1. **Add Persian Calendar Support**
   - Install: `dotnet add package PersianDateTime`
   - Use for all date displays and inputs

2. **Add Chart Library**
   - Install: `dotnet add package Blazor.Extensions.Canvas` or similar
   - Implement dashboard analytics charts

3. **Add More Validation**
   - FluentValidation for command validation
   - Client-side validation in Blazor forms

4. **Add Authentication & Authorization**
   - Implement JWT authentication
   - Add role-based access control for management panel

5. **Add Real-time Updates**
   - Implement SignalR for live dashboard updates
   - Show real-time session durations

6. **Add Reporting**
   - Export receipts to PDF/Excel
   - Financial reports by date range
   - Customer usage reports

## Contributors

This implementation was created as part of the Gaming Center Management Panel feature for Gck (Gaming Center Kowthar).

## License

This project is part of the Gck repository. Please refer to the main repository for licensing information.
