# 🗄️ Database Setup Guide - Electronics Store on macOS

## 📋 **Overview**

Complete guide to set up and manage the Electronics Store SQLite database on macOS, including installation, configuration, and management tools.

---

## 🛠️ **Prerequisites**

### **System Requirements**
- ✅ **macOS**: 10.15 (Catalina) or later
- ✅ **.NET 9.0**: ASP.NET Core runtime
- ✅ **SQLite**: Database engine (usually pre-installed)
- ✅ **Terminal**: Command line access

### **Development Tools (Optional)**
- ✅ **DB Browser for SQLite**: GUI database management
- ✅ **Visual Studio Code**: Code editing with SQLite extensions
- ✅ **TablePlus**: Premium database management tool

---

## 📦 **Installation Steps**

### **Step 1: Verify SQLite Installation**

SQLite is pre-installed on macOS. Verify installation:

```bash
# Check SQLite version
sqlite3 --version

# Expected output: 3.x.x (version number)
```

If not installed, install via Homebrew:

```bash
# Install Homebrew (if not installed)
/bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"

# Install SQLite
brew install sqlite
```

### **Step 2: Install DB Browser for SQLite (Recommended)**

Download and install the GUI tool for database management:

```bash
# Install via Homebrew
brew install --cask db-browser-for-sqlite

# Or download from: https://sqlitebrowser.org/
```

### **Step 3: Install .NET Entity Framework Tools**

```bash
# Install EF Core tools globally
dotnet tool install --global dotnet-ef

# Verify installation
dotnet ef --version
```

---

## 🚀 **Database Setup**

### **Step 1: Navigate to Project Directory**

```bash
# Navigate to your project folder
cd "/Users/phucbao/Documents/Study/LTWNC/Đồ án /Project/ElectronicsStore"

# Verify you're in the correct directory
ls -la | grep "ElectronicsStore.csproj"
```

### **Step 2: Database Configuration**

The project uses SQLite with the following connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=electronicsstore.db"
  }
}
```

### **Step 3: Create/Update Database**

```bash
# Create initial migration (if not exists)
dotnet ef migrations add InitialCreate

# Update database with latest migrations
dotnet ef database update

# Verify database file creation
ls -la electronicsstore.db
```

### **Step 4: Seed Initial Data**

The application automatically seeds initial data on startup:

```bash
# Run the application to trigger data seeding
dotnet run

# Stop the application after seeding (Ctrl+C)
```

---

## 🔧 **Database Management**

### **Using Command Line (sqlite3)**

#### **Connect to Database**
```bash
# Open database in SQLite CLI
sqlite3 electronicsstore.db

# SQLite prompt will appear: sqlite>
```

#### **Basic Commands**
```sql
-- List all tables
.tables

-- Show table schema
.schema Orders

-- View table data
SELECT * FROM Products LIMIT 5;

-- Exit SQLite CLI
.quit
```

#### **Useful Queries**
```sql
-- Check database size
SELECT page_count * page_size as size FROM pragma_page_count(), pragma_page_size();

-- List all users
SELECT Id, Email, FirstName, LastName FROM AspNetUsers;

-- Check order statistics
SELECT Status, COUNT(*) as Count FROM Orders GROUP BY Status;

-- View product categories
SELECT Id, Name, (SELECT COUNT(*) FROM Products WHERE CategoryId = c.Id) as ProductCount 
FROM Categories c WHERE IsActive = 1;
```

### **Using DB Browser for SQLite (GUI)**

#### **Open Database**
1. Launch DB Browser for SQLite
2. Click "Open Database"
3. Navigate to project folder
4. Select `electronicsstore.db`

#### **Key Features**
- ✅ **Browse Data**: View and edit table data
- ✅ **Execute SQL**: Run custom queries
- ✅ **Database Structure**: View tables, indexes, triggers
- ✅ **Import/Export**: Backup and restore data
- ✅ **Visual Query Builder**: Create queries visually

---

## 📊 **Database Schema Overview**

### **Core Tables**

#### **Products**
```sql
CREATE TABLE Products (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Description TEXT,
    Price DECIMAL(18,2) NOT NULL,
    DiscountPrice DECIMAL(18,2),
    StockQuantity INTEGER NOT NULL,
    ImageUrl TEXT,
    Brand TEXT,
    Model TEXT,
    IsActive BOOLEAN NOT NULL DEFAULT 1,
    IsFeatured BOOLEAN NOT NULL DEFAULT 0,
    CategoryId INTEGER NOT NULL,
    CreatedAt TEXT NOT NULL,
    UpdatedAt TEXT NOT NULL,
    FOREIGN KEY (CategoryId) REFERENCES Categories(Id)
);
```

#### **Orders**
```sql
CREATE TABLE Orders (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    OrderNumber TEXT NOT NULL UNIQUE,
    OrderDate TEXT NOT NULL,
    TotalAmount DECIMAL(18,2) NOT NULL,
    ShippingCost DECIMAL(18,2) NOT NULL,
    Tax DECIMAL(18,2) NOT NULL,
    GrandTotal DECIMAL(18,2) NOT NULL,
    Status INTEGER NOT NULL, -- 0=Pending, 1=Confirmed, 2=Processing, 3=Shipped, 4=Delivered, 5=Completed, 6=Cancelled
    PaymentStatus INTEGER NOT NULL,
    UserId TEXT NOT NULL,
    -- Shipping information fields
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id)
);
```

#### **Categories**
```sql
CREATE TABLE Categories (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Description TEXT,
    ImageUrl TEXT,
    IsActive BOOLEAN NOT NULL DEFAULT 1,
    CreatedAt TEXT NOT NULL
);
```

### **Identity Tables**
- `AspNetUsers`: User accounts
- `AspNetRoles`: User roles (Admin, Customer)
- `AspNetUserRoles`: User-role relationships

### **Additional Tables**
- `CartItems`: Shopping cart items
- `OrderItems`: Order line items
- `ProductReviews`: Product reviews and ratings
- `ChatMessages`: AI chatbot conversations
- `PasswordResetTokens`: Password reset functionality

---

## 🔄 **Database Maintenance**

### **Backup Database**

```bash
# Create backup with timestamp
cp electronicsstore.db "electronicsstore_backup_$(date +%Y%m%d_%H%M%S).db"

# Or use SQLite dump
sqlite3 electronicsstore.db .dump > electronicsstore_backup.sql
```

### **Restore Database**

```bash
# Restore from backup file
cp electronicsstore_backup_20241227_143000.db electronicsstore.db

# Or restore from SQL dump
sqlite3 electronicsstore_new.db < electronicsstore_backup.sql
```

### **Database Optimization**

```sql
-- Connect to database
sqlite3 electronicsstore.db

-- Analyze database for optimization
ANALYZE;

-- Vacuum database to reclaim space
VACUUM;

-- Check database integrity
PRAGMA integrity_check;
```

---

## 🔍 **Troubleshooting**

### **Common Issues**

#### **Database Locked Error**
```bash
# Check for running processes
ps aux | grep dotnet

# Kill any running .NET processes
killall dotnet

# Or restart the application
```

#### **Migration Issues**
```bash
# Reset migrations (CAUTION: Will lose data)
rm -rf Migrations/
dotnet ef migrations add InitialCreate
dotnet ef database update

# Or update to specific migration
dotnet ef database update MigrationName
```

#### **Permission Issues**
```bash
# Check file permissions
ls -la electronicsstore.db

# Fix permissions if needed
chmod 644 electronicsstore.db
```

### **Database Recovery**

#### **Corrupt Database**
```bash
# Check database integrity
sqlite3 electronicsstore.db "PRAGMA integrity_check;"

# Attempt repair
sqlite3 electronicsstore.db ".recover" | sqlite3 electronicsstore_recovered.db
```

---

## 📈 **Performance Monitoring**

### **Query Performance**
```sql
-- Enable query timing
.timer on

-- Analyze slow queries
EXPLAIN QUERY PLAN SELECT * FROM Products WHERE CategoryId = 1;

-- Check index usage
.indices Products
```

### **Database Statistics**
```sql
-- Database size
SELECT page_count * page_size / 1024 / 1024 as 'DB Size (MB)' 
FROM pragma_page_count(), pragma_page_size();

-- Table sizes
SELECT name, COUNT(*) as 'Row Count' 
FROM sqlite_master sm 
JOIN pragma_table_info(sm.name) pti 
GROUP BY name;
```

---

## 🛡️ **Security Considerations**

### **File Permissions**
```bash
# Set appropriate permissions
chmod 600 electronicsstore.db  # Read/write for owner only
```

### **Backup Security**
```bash
# Encrypt backup files
zip -e electronicsstore_backup.zip electronicsstore.db

# Store backups in secure location
mkdir -p ~/Documents/Database_Backups
mv electronicsstore_backup.zip ~/Documents/Database_Backups/
```

---

## 📚 **Additional Resources**

### **Documentation**
- [SQLite Official Documentation](https://sqlite.org/docs.html)
- [Entity Framework Core Documentation](https://docs.microsoft.com/en-us/ef/core/)
- [DB Browser for SQLite Guide](https://sqlitebrowser.org/docs/)

### **Useful Commands Reference**
```bash
# Project commands
dotnet ef migrations list                    # List all migrations
dotnet ef migrations add MigrationName      # Add new migration
dotnet ef database update                   # Apply migrations
dotnet ef database drop                     # Drop database

# SQLite commands
.help                                        # Show all commands
.tables                                      # List tables
.schema table_name                          # Show table schema
.mode column                                # Format output in columns
.headers on                                 # Show column headers
```

**🗄️ Your Electronics Store database is now ready for development and production use on macOS!**
