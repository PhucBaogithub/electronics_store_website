# 📚 API Documentation - Electronics Store

## 📋 **Overview**

Comprehensive documentation of all API endpoints in the Electronics Store application, organized by functionality and controller.

---

## 🏠 **Home Controller APIs**

### **Order Management**

#### **POST /Home/ProcessDemoOrder**
- **Purpose**: Process a demo order for testing purposes
- **Authentication**: Required (Customer)
- **Request Body**: `DemoOrderRequest` object
- **Response**: JSON with success status and order ID
- **Usage**: Called from checkout page to create orders

#### **POST /Home/CancelOrder**
- **Purpose**: Cancel a pending order
- **Authentication**: Required (Customer)
- **Parameters**: `id` (order ID)
- **Response**: JSON with success/error message
- **Restrictions**: Only pending orders can be cancelled

#### **POST /api/Orders/{orderId}/confirm-receipt**
- **Purpose**: Customer confirms receipt of delivered order
- **Authentication**: Required (Customer)
- **Parameters**: `orderId` (path parameter)
- **Response**: JSON with success message
- **Business Logic**: Transitions order from "Delivered" to "Completed"

### **Page Rendering**

#### **GET /Home/Index**
- **Purpose**: Render homepage with featured products
- **Authentication**: None
- **Response**: Homepage view with product carousel

#### **GET /Home/Products**
- **Purpose**: Render products listing page
- **Authentication**: None
- **Response**: Products page view

#### **GET /Home/ProductDetail/{id}**
- **Purpose**: Render product detail page
- **Authentication**: None
- **Parameters**: `id` (product ID)
- **Response**: Product detail view

#### **GET /Home/Orders**
- **Purpose**: Display customer's order history
- **Authentication**: Required (Customer)
- **Response**: Orders list view with order items

#### **GET /Home/OrderDetail/{id}**
- **Purpose**: Display detailed order information
- **Authentication**: Required (Customer)
- **Parameters**: `id` (order ID)
- **Response**: Order detail view with items and actions

---

## 🛍️ **Products Controller APIs**

### **Product Management**

#### **GET /api/Products**
- **Purpose**: Get paginated list of active products
- **Authentication**: None
- **Query Parameters**: 
  - `page` (default: 1)
  - `pageSize` (default: 12)
  - `categoryId` (optional)
  - `searchTerm` (optional)
  - `sortBy` (optional)
- **Response**: `ProductListResponse` with products and pagination info

#### **GET /api/Products/{id}**
- **Purpose**: Get detailed product information
- **Authentication**: None
- **Parameters**: `id` (product ID)
- **Response**: `ProductDetailDTO` object

#### **GET /api/Products/{id}/related**
- **Purpose**: Get related products from same category
- **Authentication**: None
- **Parameters**: 
  - `id` (product ID)
  - `count` (query parameter, default: 6)
- **Response**: Array of `ProductDTO` objects
- **Logic**: Returns products from same category, excluding current product

#### **GET /api/Products/featured**
- **Purpose**: Get featured products for homepage
- **Authentication**: None
- **Query Parameters**: `count` (default: 8)
- **Response**: Array of featured `ProductDTO` objects

### **Admin Product Management**

#### **POST /api/Products**
- **Purpose**: Create new product
- **Authentication**: Required (Admin)
- **Request Body**: `CreateProductDTO`
- **Response**: Created `ProductDTO`

#### **PUT /api/Products/{id}**
- **Purpose**: Update existing product
- **Authentication**: Required (Admin)
- **Parameters**: `id` (product ID)
- **Request Body**: `UpdateProductDTO`
- **Response**: Updated `ProductDTO`

#### **DELETE /api/Products/{id}**
- **Purpose**: Soft delete product (set IsActive = false)
- **Authentication**: Required (Admin)
- **Parameters**: `id` (product ID)
- **Response**: Success message

---

## 🛒 **Cart Controller APIs**

### **Cart Management**

#### **GET /api/Cart**
- **Purpose**: Get current user's cart items
- **Authentication**: Required (Customer)
- **Response**: Array of `CartItemDTO` objects

#### **POST /api/Cart/add**
- **Purpose**: Add product to cart
- **Authentication**: Required (Customer)
- **Request Body**: `AddToCartDTO` (productId, quantity)
- **Response**: Success message and updated cart count

#### **PUT /api/Cart/update**
- **Purpose**: Update cart item quantity
- **Authentication**: Required (Customer)
- **Request Body**: `UpdateCartDTO` (productId, quantity)
- **Response**: Success message

#### **DELETE /api/Cart/remove/{productId}**
- **Purpose**: Remove item from cart
- **Authentication**: Required (Customer)
- **Parameters**: `productId`
- **Response**: Success message

#### **DELETE /api/Cart/clear**
- **Purpose**: Clear all items from cart
- **Authentication**: Required (Customer)
- **Response**: Success message

#### **GET /api/Cart/count**
- **Purpose**: Get total number of items in cart
- **Authentication**: Required (Customer)
- **Response**: Integer count

---

## ⭐ **Reviews Controller APIs**

### **Review Management**

#### **GET /api/Reviews/product/{productId}**
- **Purpose**: Get all reviews for a product
- **Authentication**: None
- **Parameters**: `productId`
- **Response**: Array of `ReviewDTO` objects with user information

#### **POST /api/Reviews**
- **Purpose**: Create new product review
- **Authentication**: Required (Customer)
- **Request Body**: `CreateReviewDTO` (productId, rating, comment)
- **Response**: Created `ReviewDTO`
- **Validation**: User must have purchased and received the product

#### **PUT /api/Reviews/{id}**
- **Purpose**: Update existing review
- **Authentication**: Required (Review Owner)
- **Parameters**: `id` (review ID)
- **Request Body**: `UpdateReviewDTO`
- **Response**: Updated `ReviewDTO`

#### **DELETE /api/Reviews/{id}**
- **Purpose**: Delete review
- **Authentication**: Required (Review Owner or Admin)
- **Parameters**: `id` (review ID)
- **Response**: Success message

---

## 📂 **Categories Controller APIs**

### **Category Management**

#### **GET /api/Categories**
- **Purpose**: Get all active categories with product counts
- **Authentication**: None
- **Response**: Array of `CategoryDTO` objects

#### **GET /api/Categories/{id}**
- **Purpose**: Get category details
- **Authentication**: None
- **Parameters**: `id` (category ID)
- **Response**: `CategoryDTO` object

#### **GET /api/Categories/{id}/products**
- **Purpose**: Get products in specific category
- **Authentication**: None
- **Parameters**: `id` (category ID)
- **Query Parameters**: `page`, `pageSize`
- **Response**: Paginated product list

### **Admin Category Management**

#### **POST /api/Categories**
- **Purpose**: Create new category
- **Authentication**: Required (Admin)
- **Request Body**: `CreateCategoryDTO`
- **Response**: Created `CategoryDTO`

#### **PUT /api/Categories/{id}**
- **Purpose**: Update category
- **Authentication**: Required (Admin)
- **Parameters**: `id` (category ID)
- **Request Body**: `UpdateCategoryDTO`
- **Response**: Updated `CategoryDTO`

#### **DELETE /api/Categories/{id}**
- **Purpose**: Soft delete category
- **Authentication**: Required (Admin)
- **Parameters**: `id` (category ID)
- **Response**: Success message

---

## 👤 **Account Controller APIs**

### **Authentication**

#### **POST /Account/Login**
- **Purpose**: User login
- **Authentication**: None
- **Request Body**: Email and password
- **Response**: Redirect to dashboard or return URL

#### **POST /Account/Logout**
- **Purpose**: User logout
- **Authentication**: Required
- **Response**: Redirect to homepage

#### **POST /Account/Register**
- **Purpose**: User registration
- **Authentication**: None
- **Request Body**: User registration data
- **Response**: Redirect to login or dashboard

### **Profile Management**

#### **GET /Account/Profile**
- **Purpose**: Display user profile
- **Authentication**: Required
- **Response**: Profile view with user information

#### **POST /Account/UpdateProfile**
- **Purpose**: Update user profile
- **Authentication**: Required
- **Request Body**: Updated user information
- **Response**: Success/error message

---

## 🔐 **Admin Controller APIs**

### **Dashboard**

#### **GET /Admin/Index**
- **Purpose**: Admin dashboard with statistics
- **Authentication**: Required (Admin)
- **Response**: Dashboard view with metrics

### **Order Management**

#### **GET /Admin/Orders**
- **Purpose**: View all orders
- **Authentication**: Required (Admin)
- **Response**: Orders list view

#### **POST /Admin/UpdateOrderStatus**
- **Purpose**: Update order status
- **Authentication**: Required (Admin)
- **Request Body**: `orderId`, `status`
- **Response**: Success/error message
- **Note**: Cannot set "Completed" status (customer-only)

### **User Management**

#### **GET /Admin/Users**
- **Purpose**: View all users
- **Authentication**: Required (Admin)
- **Response**: Users list with roles

---

## 💬 **Chat Controller APIs**

### **AI Chatbot**

#### **POST /api/Chat/send**
- **Purpose**: Send message to AI chatbot
- **Authentication**: Optional
- **Request Body**: `ChatMessageDTO`
- **Response**: AI response with product recommendations

#### **GET /api/Chat/history/{conversationId}**
- **Purpose**: Get chat conversation history
- **Authentication**: Optional
- **Parameters**: `conversationId`
- **Response**: Array of chat messages

---

## 🔄 **Common Response Formats**

### **Success Response**
```json
{
    "success": true,
    "message": "Operation completed successfully",
    "data": { /* response data */ }
}
```

### **Error Response**
```json
{
    "success": false,
    "message": "Error description",
    "errors": ["Detailed error messages"]
}
```

### **Pagination Response**
```json
{
    "items": [/* array of items */],
    "totalCount": 100,
    "pageNumber": 1,
    "pageSize": 12,
    "totalPages": 9
}
```

---

## 🔒 **Authentication & Authorization**

### **Authentication Methods**
- **Cookie Authentication**: ASP.NET Core Identity
- **Role-based Authorization**: Admin, Customer roles

### **Protected Endpoints**
- All `/Admin/*` endpoints require Admin role
- Cart operations require Customer authentication
- Order operations require ownership validation
- Review operations require purchase validation

---

## 📊 **Status Codes**

- **200 OK**: Successful operation
- **201 Created**: Resource created successfully
- **400 Bad Request**: Invalid request data
- **401 Unauthorized**: Authentication required
- **403 Forbidden**: Insufficient permissions
- **404 Not Found**: Resource not found
- **500 Internal Server Error**: Server error

**🚀 All APIs are production-ready with proper error handling, validation, and security measures!**
