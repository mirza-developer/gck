# GCK User Management System

A comprehensive user management system built with Clean Architecture principles, CQRS pattern, authentication, and modern .NET technologies.

## Architecture

This project follows Clean Architecture with clear separation of concerns:

```
gck/
├── Gck.Domain/          # Domain entities and business rules
├── Gck.Persistence/     # Data access layer with EF Core
├── Gck.Application/     # Application logic with CQRS
├── Gck.Api/            # REST API layer
├── Gck/                # Blazor WebAssembly UI
└── README.md
```

## Technologies Used

- **.NET 9.0**
- **Entity Framework Core 9.0** - Database ORM
- **MediatR 12.4.1** - CQRS pattern implementation
- **AutoMapper 13.0.1** - Object mapping
- **FluentValidation 11.9.2** - Input validation
- **SQL Server** - Database
- **Blazor WebAssembly** - Frontend UI
- **Swagger/OpenAPI** - API documentation

## Features

### Authentication & Security
- ✅ **Login System** with username/password authentication
- ✅ **Default Admin User** (admin/Admin@123) seeded automatically
- ✅ **Session Management** using LocalStorage
- ✅ **Protected Routes** - Users menu visible only to authenticated users
- ✅ **Logout Functionality** with session cleanup

### User Management
- ✅ Create new users with validation
- ✅ Update user information
- ✅ Delete users
- ✅ Change user passwords
- ✅ Activate/deactivate users
- ✅ Search and filter users
- ✅ User claims management

### Security
- ✅ Password hashing using PBKDF2
- ✅ Input validation with FluentValidation
- ✅ Parameterized queries (SQL injection protection)
- ✅ RESTful API with proper HTTP status codes

## Getting Started

### Prerequisites

- .NET 9.0 SDK or later
- SQL Server (LocalDB, Express, or Full)
- Visual Studio 2022 (optional) or VS Code

### Database Setup

1. **Update Connection String** (if needed)
   
   Edit `Gck.Api/appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=.;Database=GckDb;Trusted_Connection=True;TrustServerCertificate=True;"
     }
   }
   ```

2. **Apply Migrations**

   The database will be automatically initialized and seeded with a default admin user when you run the API for the first time.

   Or manually apply migrations:
   ```bash
   cd Gck.Persistence
   dotnet ef database update
   ```

   Or from solution root:
   ```bash
   dotnet ef database update --project Gck.Persistence --startup-project Gck.Api
   ```

### Default Admin Credentials

After first run, a default admin user is automatically created:
- **Username**: `admin`
- **Password**: `Admin@123`

This allows immediate access to the system for testing and configuration.

### Running the Application

#### Option 1: Run API and UI Separately

**Terminal 1 - API:**
```bash
cd Gck.Api
dotnet run
```
API will be available at: `https://localhost:7023`
Swagger UI: `https://localhost:7023/swagger`

**Terminal 2 - Blazor UI:**
```bash
cd Gck
dotnet run
```
UI will be available at: `https://localhost:5001` (or the port shown in terminal)

**Important:** The API will automatically:
- Apply any pending database migrations
- Seed the default admin user if no users exist
- Display credentials in the console logs

#### Option 2: Run with Visual Studio
1. Right-click on Solution
2. Set Multiple Startup Projects
3. Select both `Gck.Api` and `Gck`
4. Press F5

### Using the Application

1. **Access the UI**
   - Navigate to `https://localhost:5001` (or your Blazor app port)

2. **Login**
   - Click "ورود" (Login) in the navigation menu
   - Use default credentials: `admin` / `Admin@123`
   - After successful login, you'll be redirected to the home page

3. **Access User Management**
   - Once logged in, click "کاربران" (Users) in the navigation menu

4. **Add a User**
   - Click "افزودن کاربر جدید" (Add New User)
   - Fill in the required fields:
     - Username (required)
     - Name (required)
     - Password (required, min 6 characters)
     - Email (optional, must be valid format)
     - Phone Number (optional)
     - Details (optional)
   - Click "ثبت کاربر" (Submit User)

3. **Edit a User**
   - From the users list, click "ویرایش" (Edit) on any user card
   - Update the information
   - Toggle the active status if needed
   - Click "ذخیره تغییرات" (Save Changes)

4. **Delete a User**
   - From the users list, click "حذف" (Delete) on any user card
   - Confirm the deletion

5. **Logout**
   - Click "خروج" (Logout) button in the navigation bar to end your session

## API Endpoints

### Authentication

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/login` | Login with username and password |

### User Management

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/user` | Get all users |
| GET | `/api/user/{id}` | Get user by ID |
| GET | `/api/user/username/{username}` | Get user by username |
| POST | `/api/user` | Create new user |
| PUT | `/api/user/{id}` | Update user |
| DELETE | `/api/user/{id}` | Delete user |
| PUT | `/api/user/{id}/password` | Update user password |

### Example API Requests

**Login:**
```bash
curl -X POST https://localhost:7023/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin",
    "password": "Admin@123"
  }'
```

**Create User:**
```bash
curl -X POST https://localhost:7023/api/user \
  -H "Content-Type: application/json" \
  -d '{
    "username": "john.doe",
    "name": "John Doe",
    "password": "SecurePass123",
    "email": "john@example.com",
    "phoneNumber": "09123456789"
  }'
```

**Get All Users:**
```bash
curl https://localhost:7023/api/user
```

## Project Structure

### Gck.Domain
```
Entities/
├── User.cs           # User entity with data annotations
└── UserClaim.cs      # User claim entity
```

### Gck.Persistence
```
├── GckDbContext.cs          # EF Core DbContext
├── GckDbContextFactory.cs   # Design-time factory
└── Migrations/              # EF Core migrations
```

### Gck.Application
```
Features/Users/
├── Commands/
│   ├── AddUser/
│   ├── UpdateUser/
│   ├── DeleteUser/
│   └── UpdateUserPassword/
├── Queries/
│   ├── GetAllUsers/
│   ├── GetUserById/
│   └── GetUserByUsername/
DTOs/
└── UserDtos.cs
Validators/
├── AddUserCommandValidator.cs
├── UpdateUserCommandValidator.cs
└── UpdateUserPasswordCommandValidator.cs
```

### Gck.Api
```
Controllers/
└── UserController.cs        # REST API endpoints
Program.cs                   # API configuration
appsettings.json            # Configuration including connection string
```

### Gck (Blazor UI)
```
Pages/Users/
├── Index.razor             # User list page
├── Add.razor              # Add user form
├── Edit.razor             # Edit user form
└── *.css files            # Page-specific styles
```

## Database Schema

### tbl_User
| Column | Type | Constraints |
|--------|------|-------------|
| Id | string(450) | Primary Key |
| Username | string(256) | Required, Unique |
| Name | string(512) | Required |
| Email | string(256) | Nullable |
| PasswordHash | string | Required |
| IsActive | bool | Required |
| CreateDate | DateTime | Required |
| LastModifiedDate | DateTime | Required |
| CreatorIdentityID | string(128) | Required |
| LastModifierIdentityID | string(128) | Nullable |
| PhoneNumber | string | Nullable |
| Details | string | Nullable |

### tbl_UserClaim
| Column | Type | Constraints |
|--------|------|-------------|
| Id | int | Primary Key, Auto-increment |
| UserId | string(450) | Foreign Key to User, Required |
| ClaimType | string | Nullable |
| ClaimValue | string | Nullable |

## Troubleshooting

### Database Connection Issues
- Verify SQL Server is running
- Check connection string in `appsettings.json`
- Ensure database migrations are applied

### API Not Accessible
- Verify the API is running on the correct port
- Check firewall settings
- Update API URLs in Blazor pages if port differs

### Build Errors
```bash
# Clean and rebuild
dotnet clean
dotnet build
```

### Migration Issues
```bash
# Remove last migration
dotnet ef migrations remove --project Gck.Persistence

# Add new migration
dotnet ef migrations add MigrationName --project Gck.Persistence
```

## Development

### Adding a New Command

1. Create command class in `Gck.Application/Features/Users/Commands/YourCommand/`
2. Create command handler implementing `IRequestHandler<YourCommand, TResponse>`
3. Add validator in `Gck.Application/Validators/`
4. Add endpoint in `Gck.Api/Controllers/UserController.cs`

### Adding a New Query

1. Create query class in `Gck.Application/Features/Users/Queries/YourQuery/`
2. Create query handler implementing `IRequestHandler<YourQuery, TResponse>`
3. Add endpoint in `Gck.Api/Controllers/UserController.cs`

## Contributing

1. Follow Clean Architecture principles
2. Maintain CQRS pattern for commands and queries
3. Use FluentValidation for input validation
4. Keep controllers thin - business logic in handlers
5. Write meaningful commit messages
6. Update documentation for new features

## License

This project is part of the GCK Gaming Center platform.

## Support

For issues or questions:
- GitHub Issues: [Repository Issues](https://github.com/mirza-developer/gck/issues)
- Email: support@gckgames.ir

---

**Built with ❤️ for GCK Gaming Center**
