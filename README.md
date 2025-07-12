# Electronics Store

Một ứng dụng web bán hàng điện tử được xây dựng với ASP.NET Core 9.0, Entity Framework Core và SQLite.

## Tính năng chính

- **Quản lý sản phẩm**: Thêm, sửa, xóa sản phẩm với hình ảnh
- **Quản lý danh mục**: Phân loại sản phẩm theo danh mục
- **Hệ thống đánh giá**: Người dùng có thể đánh giá và nhận xét sản phẩm
- **Giỏ hàng**: Thêm sản phẩm vào giỏ hàng và thanh toán
- **Quản lý đơn hàng**: Theo dõi trạng thái đơn hàng
- **Hệ thống phân quyền**: Admin và User với quyền hạn khác nhau
- **Giao diện responsive**: Tương thích với mọi thiết bị

## Công nghệ sử dụng

- **Backend**: ASP.NET Core 9.0
- **Database**: SQLite với Entity Framework Core
- **Authentication**: ASP.NET Core Identity
- **Frontend**: HTML, CSS, JavaScript, Bootstrap 5
- **Icons**: Font Awesome
- **Architecture**: MVC Pattern

## Yêu cầu hệ thống

- .NET 9.0 SDK
- Visual Studio 2022 hoặc VS Code
- SQLite

## Cài đặt và chạy

### 1. Clone repository
```bash
git clone https://github.com/PhucBaogithub/electronics_store_website.git
cd ElectronicsStore
```

### 2. Restore packages
```bash
dotnet restore
```

### 3. Chạy migrations (nếu cần)
```bash
dotnet ef database update
```

### 4. Chạy ứng dụng
```bash
dotnet run
```

Ứng dụng sẽ chạy tại: `https://localhost:5221`

## Tài khoản test

Để sử dụng ứng dụng, bạn có thể đăng nhập bằng các tài khoản sau:

### Tài khoản Admin
- **Email**: `admin@electronicsstore.com`
- **Mật khẩu**: `Admin123!`
- **Quyền hạn**: 
  - Quản lý tất cả sản phẩm
  - Quản lý danh mục
  - Quản lý đơn hàng
  - Quản lý người dùng
  - Quản lý đánh giá
  - Xem dashboard thống kê

### Tài khoản User
- **Email**: `user@test.com`
- **Mật khẩu**: `Test123!`
- **Quyền hạn**:
  - Xem và mua sản phẩm
  - Quản lý giỏ hàng
  - Theo dõi đơn hàng
  - Đánh giá sản phẩm
  - Quản lý thông tin cá nhân

## Cấu trúc dự án

```
ElectronicsStore/
├── Controllers/           # API Controllers
├── Models/               # Data Models & DTOs
├── Views/                # Razor Views
├── Data/                 # Database Context
├── Migrations/           # EF Core Migrations
├── wwwroot/             # Static files (CSS, JS, Images)
└── Properties/          # Launch settings
```

## Hướng dẫn sử dụng

### Cho Admin:
1. Đăng nhập với tài khoản admin
2. Truy cập Dashboard để xem thống kê
3. Quản lý sản phẩm: Thêm/sửa/xóa sản phẩm
4. Quản lý danh mục: Tạo và chỉnh sửa danh mục
5. Theo dõi đơn hàng và quản lý người dùng

### Cho User:
1. Đăng nhập hoặc đăng ký tài khoản mới
2. Duyệt sản phẩm theo danh mục
3. Thêm sản phẩm vào giỏ hàng
4. Thanh toán và theo dõi đơn hàng
5. Đánh giá sản phẩm đã mua

## Database Schema

Ứng dụng sử dụng 13 bảng chính:

- **Identity Tables**: AspNetUsers, AspNetRoles, AspNetUserRoles...
- **Business Tables**: Products, Categories, Orders, OrderItems, CartItems, ProductReviews

## Tính năng kỹ thuật

- **Soft Delete**: Sản phẩm không bị xóa vĩnh viễn
- **Image Upload**: Upload và quản lý hình ảnh sản phẩm
- **Pagination**: Phân trang cho danh sách sản phẩm
- **Search**: Tìm kiếm sản phẩm theo tên
- **Responsive Design**: Giao diện thích ứng với mobile
- **Error Handling**: Xử lý lỗi toàn diện
- **Security**: Bảo mật với ASP.NET Core Identity

## Ghi chú

- Database SQLite được tạo tự động khi chạy lần đầu
- Hình ảnh sản phẩm được lưu trong thư mục `wwwroot/images/products/`
- Ứng dụng hỗ trợ Vietnamese locale

## Đóng góp

Mọi đóng góp đều được hoan nghênh! Hãy tạo pull request hoặc báo cáo bug.

## License

Dự án này được phát hành dưới giấy phép MIT.

## Author
Name: [Phúc Bảo]
Email: [baominecraft12344@gmail.com]

---

**Developed with ❤️ using ASP.NET Core** 
