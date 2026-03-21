# HourlyFee Management Testing Guide

This document provides instructions for testing the newly implemented HourlyFee entity management functionality.

## Prerequisites

1. .NET 9.0 SDK installed
2. SQL Server (LocalDB, Express, or Full edition)
3. The application built successfully

## Database Setup

The HourlyFee table is already included in the initial migration. When you run the API for the first time, it will automatically:
1. Apply any pending migrations
2. Seed the default hourly fees (if the table is empty):
   - 2 people: 150,000 Toman
   - 3 people: 200,000 Toman
   - 4 people: 250,000 Toman
   - 5 people: 300,000 Toman

## Testing Steps

### 1. Start the API

```bash
cd Gck.Api
dotnet run
```

The API will be available at: `https://localhost:7023`

### 2. Start the Blazor UI

In a separate terminal:

```bash
cd Gck
dotnet run
```

The UI will be available at: `http://localhost:5193`

### 3. Test Home Page Pricing Table

1. Navigate to the home page: `http://localhost:5193`
2. Scroll down to the "نرخ‌های ساعتی ما" (Our Hourly Rates) section
3. Verify that the pricing table displays the hourly fees dynamically loaded from the database
4. The table should show:
   - Number of people (تعداد نفر)
   - Price per hour (قیمت هر ساعت)

**Expected Result**: The pricing table should display all hourly fees from the database with alternating row colors.

### 4. Test HourlyFee Management Pages

#### Access Management Page
1. Navigate to: `http://localhost:5193/hourlyfees`
2. You should see the HourlyFee management page with a list of all hourly fees

**Expected Result**: A table showing all hourly fees with columns for:
- Number of people
- Price per hour
- Actions (Edit and Delete buttons)

#### Test Search Functionality
1. Type in the search box at the top of the page
2. Try searching for a number (e.g., "2" or "150000")

**Expected Result**: The table filters to show only matching records.

#### Test Add New HourlyFee
1. Click the "افزودن نرخ ساعتی جدید" (Add New Hourly Fee) button
2. Fill in the form:
   - Number of People: 6
   - Price Per Hour: 350000
3. Click "ذخیره" (Save)

**Expected Result**: 
- Success notification appears
- Redirected to the list page
- New hourly fee appears in the list

#### Test Edit HourlyFee
1. Click the "ویرایش" (Edit) button on any hourly fee
2. Modify the values:
   - Change the number of people or price
3. Click "ذخیره" (Save)

**Expected Result**:
- Success notification appears
- Redirected to the list page
- Updated values are reflected in the list

#### Test Delete HourlyFee
1. Click the "حذف" (Delete) button on any hourly fee
2. The item should be deleted immediately

**Expected Result**:
- Success notification appears
- The hourly fee is removed from the list

### 5. Test API Endpoints Directly

You can test the API endpoints using Swagger UI at: `https://localhost:7023/swagger`

#### Available Endpoints:

1. **GET /api/hourlyfees** - Get all hourly fees
2. **GET /api/hourlyfees/{id}** - Get hourly fee by ID
3. **POST /api/hourlyfees** - Create new hourly fee
   ```json
   {
     "seatsCount": 6,
     "fee": 350000
   }
   ```
4. **PUT /api/hourlyfees/{id}** - Update existing hourly fee
   ```json
   {
     "id": 1,
     "seatsCount": 6,
     "fee": 360000
   }
   ```
5. **DELETE /api/hourlyfees/{id}** - Delete hourly fee

### 6. Verify Home Page Updates

After adding, editing, or deleting hourly fees:
1. Navigate back to the home page
2. Scroll to the pricing section
3. Refresh the page if needed

**Expected Result**: The pricing table should reflect the current state of hourly fees in the database.

## Validation Testing

### Test Required Fields
1. Navigate to Add or Edit page
2. Try to submit the form without filling in all fields

**Expected Result**: Validation errors should appear for required fields.

### Test Negative Values
1. Try to enter negative numbers for seats count or fee

**Expected Result**: Should not allow negative values (HTML5 number input validation).

## Integration Points

The HourlyFee entity integrates with:
1. **Home Page**: Pricing table displays hourly fees
2. **Sessions**: HourlyFee entities are referenced by Session entities through the HourlyFeeId foreign key

## Troubleshooting

### Database Not Found
- Ensure SQL Server is running
- Check the connection string in `Gck.Api/appsettings.json`
- The default connection string is: `Server=.;Database=GckDb;Trusted_Connection=True;TrustServerCertificate=True;`

### API Connection Error
- Verify the API is running on port 5200 (http) or 7023 (https)
- Check `Gck/Services/ApiConfigurationService.cs` for the configured API base URL

### No Data Displayed on Home Page
- Check browser console for any errors
- Verify the API is responding to `/api/hourlyfees` endpoint
- Ensure hourly fees exist in the database

## Architecture Verification

The implementation follows the established patterns:

1. **Clean Architecture**: Separation of concerns across Domain, Application, Persistence, API, and UI layers
2. **CQRS Pattern**: Commands for mutations (Create, Update, Delete) and Queries for reads (GetAll, GetById)
3. **MediatR**: All operations go through MediatR handlers
4. **Entity Framework Core**: Database access through GckDbContext
5. **Blazor WebAssembly**: Frontend using Blazor components with code-behind pattern
6. **Persian Localization**: All UI text uses PersianResources for localization

## Success Criteria

✅ All CRUD operations work correctly  
✅ Home page pricing table displays dynamic data  
✅ Search and filtering work properly  
✅ Validation prevents invalid data entry  
✅ Success/error notifications display correctly  
✅ Navigation between pages works smoothly  
✅ Data persists across application restarts  
✅ Build completes without errors  
✅ No security vulnerabilities detected  

## Next Steps

After testing, you may want to:
1. Add validation rules for business constraints (e.g., minimum/maximum seat count)
2. Add authorization to restrict access to management pages
3. Add audit logging for CRUD operations
4. Consider caching for the home page pricing table
5. Add unit and integration tests
