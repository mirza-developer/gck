# Loyalty Program Feature Documentation

## Overview
The loyalty program feature allows gaming centers to reward frequent customers with free sessions after they complete a specified number of paid sessions.

## Features Implemented

### 1. Customer Entity Enhancements
Added three new properties to the `Customer` entity:
- **IsLoyal** (bool): Indicates if the customer is enrolled in the loyalty program
- **SessionsRequiredForFree** (int): Number of paid sessions required before earning a free session
- **PaidSessionsCount** (int): Current count of paid sessions completed by the customer

### 2. Session Entity Enhancement
Added one new property to the `Session` entity:
- **IsFreeSession** (bool): Indicates if this session was redeemed as a free loyalty session

### 3. Loyalty Service
Created `ILoyaltyService` with the following methods:
- **CanCustomerGetFreeSession(customerId)**: Checks if customer has earned a free session
- **IncrementPaidSessions(customerId)**: Increments the paid sessions counter
- **ResetPaidSessionsCount(customerId)**: Resets the counter after redeeming a free session

### 4. UI Enhancements

#### Customers Index Page
- Added "Loyalty Program" column showing:
  - **Green Badge**: "???? ?????? ????? ???" (Free Session Available) - when customer qualifies
  - **Gold Badge**: Shows remaining sessions until free (e.g., "3 ???? ?? ??????")
  - **Regular Text**: "????" (Regular Customer) - for non-loyal customers
- Expanded details panel shows:
  - Loyalty status
  - Sessions required for free
  - Current paid sessions count

#### Add/Edit Customer Forms
- Added checkbox: "????????? ?????? ???????" (Enable Loyalty Program)
- Conditional field: "????? ???? ???? ?? ???? ??????" (Sessions Required for Free)
- Form automatically shows/hides sessions field based on loyalty checkbox

### 5. Persian Resources
Added comprehensive Persian labels:
- `IsLoyal`: "????? ??????"
- `LoyaltyProgram`: "?????? ???????"
- `SessionsRequiredForFree`: "????? ???? ???? ?? ???? ??????"
- `PaidSessionsCount`: "????? ???????? ?????? ???"
- `FreeSessionAvailable`: "???? ?????? ????? ???"
- `SessionsUntilFree`: "???? ?? ??????"
- `LoyalCustomer`: "??????"
- `RegularCustomer`: "????"
- `EnableLoyaltyProgram`: "????????? ?????? ???????"
- `SessionsRequiredPlaceholder`: "????: 10"

### 6. CSS Styling
Added badge classes for loyalty indicators:
- `.grid-cell-badge.free-session`: Green badge for free sessions available
- `.grid-cell-badge.loyal-customer`: Gold badge for loyal customer status

## How It Works

### Setting Up a Loyal Customer
1. Navigate to **Customers > Add New Customer** or edit an existing customer
2. Check the **"????????? ?????? ???????"** checkbox
3. Enter the number of sessions required (e.g., 10) in **"????? ???? ???? ?? ???? ??????"**
4. Save the customer

### Tracking Progress
The loyalty program automatically tracks:
- Every paid session increments `PaidSessionsCount`
- When `PaidSessionsCount >= SessionsRequiredForFree`, customer earns a free session
- Free sessions are clearly indicated in the UI

### Redeeming Free Sessions
When creating a new session:
1. System checks if customer qualifies for free session
2. If qualified, session can be marked as `IsFreeSession = true`
3. After redeeming, `PaidSessionsCount` resets to 0
4. Customer starts accumulating sessions again

## Database Migrations

Two migrations were created:
1. **AddLoyaltyProgramToCustomer**: Adds loyalty fields to Customer table
2. **AddIsFreeSessionToSession**: Adds IsFreeSession field to Session table

Run migrations with:
```bash
dotnet ef database update --project Gck.Persistence --startup-project Gck.Api
```

## API Changes

### Customer DTOs Updated
- `CustomerDto`: Added IsLoyal, SessionsRequiredForFree, PaidSessionsCount
- `CreateCustomerDto`: Added IsLoyal, SessionsRequiredForFree
- `UpdateCustomerDto`: Added IsLoyal, SessionsRequiredForFree

### Customer Commands Updated
- `CreateCustomerCommand`: Now accepts loyalty program fields
- `UpdateCustomerCommand`: Now accepts loyalty program fields
- Handlers updated to persist loyalty data

### Customer Queries Updated
- `GetAllCustomers`: Returns loyalty information
- `GetCustomerById`: Returns loyalty information

## Usage Example

### Creating a Loyal Customer
```json
POST /api/customers
{
  "name": "??? ?????",
  "phoneNumber": "09123456789",
  "birthYear": 1985,
  "gender": "Male",
  "isLoyal": true,
  "sessionsRequiredForFree": 10
}
```

### Checking Loyalty Status
```csharp
var canGetFree = await loyaltyService.CanCustomerGetFreeSession(customerId);
if (canGetFree)
{
    // Offer free session
    // Create session with IsFreeSession = true
    await loyaltyService.ResetPaidSessionsCount(customerId);
}
```

### Incrementing Sessions
```csharp
// After completing a paid session
await loyaltyService.IncrementPaidSessions(customerId);
```

## Visual Indicators

### Grid Display
```
| ???    | ????         | ????? ??????? | ?????? ???????          |
|--------|--------------|---------------|-------------------------|
| ???    | 09123456789  | 12 ???        | ?? ???? ?????? ?????    |
| ????   | 09121234567  | 5 ???         | ? 5 ???? ?? ??????     |
| ????   | 09129876543  | 8 ???         | ????                    |
```

### Expanded Details
When expanding a loyal customer row:
- ? ????? ??????: ??????
- ?? ????? ???? ???? ?? ???? ??????: 10 ???
- ?? ????? ???????? ?????? ???: 12 ???

## Future Enhancements (Optional)

1. **Auto-apply free session**: Automatically mark next session as free when customer qualifies
2. **Loyalty history**: Track history of free sessions redeemed
3. **Tiered loyalty**: Different rewards based on customer tiers (bronze, silver, gold)
4. **Expiration dates**: Add expiration to earned free sessions
5. **Email notifications**: Notify customers when they earn a free session
6. **Analytics dashboard**: Show loyalty program statistics and effectiveness

## Testing Checklist

- [ ] Create a loyal customer with 5 sessions required
- [ ] Verify loyalty status shows in grid
- [ ] Complete 5 sessions and check for "free session available" indicator
- [ ] Edit customer to change sessions required
- [ ] Disable loyalty program for a customer
- [ ] Verify responsive display on mobile devices
- [ ] Test with Persian text display
- [ ] Verify database migrations applied successfully
- [ ] Check API endpoints return loyalty data correctly

## Support

For issues or questions about the loyalty program:
- Check database migrations are applied
- Verify LoyaltyService is registered in DI
- Ensure API and UI are using latest code
- Review browser console for any JavaScript errors
