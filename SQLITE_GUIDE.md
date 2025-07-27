# Electronics Store - SQLite Database Guide

## Cài đặt SQLite và GUI Tools trên macOS

### 1. Cài đặt SQLite
```bash
# SQLite đã có sẵn trên macOS, kiểm tra version:
sqlite3 --version

# Hoặc cài đặt version mới nhất qua Homebrew:
brew install sqlite
```

### 2. Cài đặt Entity Framework CLI Tools
```bash
# Cài đặt EF Core tools globally
dotnet tool install --global dotnet-ef

# Kiểm tra cài đặt
dotnet ef --version
```

### 3. Cài đặt GUI Tools

#### DB Browser for SQLite (Khuyến nghị - Miễn phí)
```bash
brew install --cask db-browser-for-sqlite
```

#### TablePlus (Có phí)
```bash
brew install --cask tableplus
```

#### SQLiteStudio (Miễn phí)
```bash
brew install --cask sqlitestudio
```

#### DBeaver (Miễn phí)
```bash
brew install --cask dbeaver-community
```

## Sử dụng Database

### 1. Chạy ứng dụng để tạo database
```bash
cd "/Users/phucbao/Documents/Study/LTWNC/Đồ án /Project/ElectronicsStore"
dotnet run
```

### 2. Kết nối với database

#### Sử dụng script tự động:
```bash
# Xem thông tin database
./connect_db.sh info

# Kết nối database local
./connect_db.sh local

# Kết nối database từ Docker
./connect_db.sh docker
```

#### Kết nối thủ công:
1. Mở **DB Browser for SQLite**
2. Click **"Open Database"**
3. Navigate đến file `ElectronicsStore.db`
4. Click **"Open"**

### 3. Cấu trúc Database

#### Tables chính:
- **AspNetUsers** - Thông tin người dùng
- **AspNetRoles** - Vai trò người dùng
- **Categories** - Danh mục sản phẩm
- **Products** - Sản phẩm
- **Orders** - Đơn hàng
- **OrderItems** - Chi tiết đơn hàng
- **CartItems** - Giỏ hàng
- **PasswordResetTokens** - Token reset mật khẩu

## Entity Framework Commands

### Migration Commands
```bash
# Tạo migration mới
dotnet ef migrations add MigrationName

# Áp dụng migrations
dotnet ef database update

# Xem danh sách migrations
dotnet ef migrations list

# Tạo SQL script từ migrations
dotnet ef migrations script

# Rollback đến migration cụ thể
dotnet ef database update PreviousMigrationName

# Xóa migration chưa apply
dotnet ef migrations remove
```

### Database Commands
```bash
# Xóa database
dotnet ef database drop

# Tạo lại database từ đầu
dotnet ef database drop --force
dotnet ef database update
```

## SQLite Command Line

### Kết nối và truy vấn cơ bản
```bash
# Mở database
sqlite3 ElectronicsStore.db

# Trong SQLite shell:
.tables                    # Xem tất cả tables
.schema Products          # Xem schema của table
.headers on               # Hiển thị headers
.mode column              # Format output đẹp
.width 10 20 30           # Set column widths

# Truy vấn dữ liệu
SELECT * FROM Products LIMIT 10;
SELECT * FROM Categories;
SELECT COUNT(*) FROM AspNetUsers;

# Thoát
.quit
```

### Backup và Restore
```bash
# Backup database
sqlite3 ElectronicsStore.db ".backup backup_$(date +%Y%m%d_%H%M%S).db"

# Backup specific table
sqlite3 ElectronicsStore.db ".dump Products" > products_backup.sql

# Restore database
sqlite3 new_database.db ".restore backup_20250127_123456.db"

# Import SQL file
sqlite3 ElectronicsStore.db < products_backup.sql
```

## Useful Queries

### User Management
```sql
-- Xem tất cả users
SELECT Id, FirstName, LastName, Email, IsActive, CreatedAt 
FROM AspNetUsers;

-- Xem users theo role
SELECT u.FirstName, u.LastName, u.Email, r.Name as Role
FROM AspNetUsers u
JOIN AspNetUserRoles ur ON u.Id = ur.UserId
JOIN AspNetRoles r ON ur.RoleId = r.Id;

-- Đếm users theo role
SELECT r.Name, COUNT(*) as UserCount
FROM AspNetRoles r
LEFT JOIN AspNetUserRoles ur ON r.Id = ur.RoleId
GROUP BY r.Name;
```

### Product Management
```sql
-- Xem products với categories
SELECT p.Name, p.Price, p.Stock, c.Name as Category
FROM Products p
JOIN Categories c ON p.CategoryId = c.Id
ORDER BY p.CreatedAt DESC;

-- Products hết hàng
SELECT Name, Price, Stock
FROM Products
WHERE Stock <= 0;

-- Top selling products
SELECT p.Name, SUM(oi.Quantity) as TotalSold
FROM Products p
JOIN OrderItems oi ON p.Id = oi.ProductId
GROUP BY p.Id, p.Name
ORDER BY TotalSold DESC
LIMIT 10;
```

### Order Analysis
```sql
-- Orders theo status
SELECT Status, COUNT(*) as OrderCount, SUM(TotalAmount) as TotalRevenue
FROM Orders
GROUP BY Status;

-- Revenue theo tháng
SELECT strftime('%Y-%m', CreatedAt) as Month, 
       COUNT(*) as OrderCount,
       SUM(TotalAmount) as Revenue
FROM Orders
WHERE Status = 'Completed'
GROUP BY strftime('%Y-%m', CreatedAt)
ORDER BY Month DESC;

-- Top customers
SELECT u.FirstName, u.LastName, u.Email,
       COUNT(o.Id) as OrderCount,
       SUM(o.TotalAmount) as TotalSpent
FROM AspNetUsers u
JOIN Orders o ON u.Id = o.UserId
GROUP BY u.Id
ORDER BY TotalSpent DESC
LIMIT 10;
```

## Troubleshooting

### Common Issues

1. **Database locked**
```bash
# Kiểm tra processes đang sử dụng database
lsof ElectronicsStore.db

# Kill process nếu cần
kill -9 <process_id>
```

2. **Permission denied**
```bash
# Fix permissions
chmod 644 ElectronicsStore.db
```

3. **Database not found**
```bash
# Chạy ứng dụng để tạo database
dotnet run

# Hoặc tạo migrations
dotnet ef database update
```

4. **Migration errors**
```bash
# Reset migrations
rm -rf Migrations/
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Performance Tips

1. **Indexes** - Tạo indexes cho columns thường query
2. **VACUUM** - Dọn dẹp database định kỳ
3. **ANALYZE** - Cập nhật statistics cho query optimizer

```sql
-- Vacuum database
VACUUM;

-- Analyze tables
ANALYZE;

-- Check database size
SELECT page_count * page_size as size FROM pragma_page_count(), pragma_page_size();
```

## Security Notes

- Database file nên có permissions 644
- Backup database thường xuyên
- Không commit database file vào Git
- Sử dụng environment variables cho connection strings trong production
