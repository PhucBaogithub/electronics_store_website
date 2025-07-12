// ===== GLOBAL VARIABLES =====
const API_BASE_URL = window.location.origin + '/api';
let currentUser = null;
let cartCount = 0;

// ===== UTILITY FUNCTIONS =====
function showLoading() {
    document.getElementById('loadingOverlay').style.display = 'flex';
}

function hideLoading() {
    document.getElementById('loadingOverlay').style.display = 'none';
}

function showToast(message, type = 'success') {
    const toast = document.getElementById('toastMessage');
    const toastBody = document.getElementById('toastBody');
    
    // Update toast content
    toastBody.textContent = message;
    
    // Update toast styling based on type
    toast.className = `toast ${type === 'success' ? 'bg-success text-white' : type === 'danger' ? 'bg-danger text-white' : 'bg-info text-white'}`;
    
    // Show toast
    const bsToast = new bootstrap.Toast(toast);
    bsToast.show();
}

function formatCurrency(amount) {
    return new Intl.NumberFormat('en-US', {
        style: 'currency',
        currency: 'USD'
    }).format(amount);
}

function formatDate(dateString) {
    return new Intl.DateTimeFormat('en-US', {
        year: 'numeric',
        month: 'long',
        day: 'numeric'
    }).format(new Date(dateString));
}

function debounce(func, wait) {
    let timeout;
    return function executedFunction(...args) {
        const later = () => {
            clearTimeout(timeout);
            func(...args);
        };
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
    };
}

// ===== API HELPER FUNCTIONS =====
async function apiRequest(url, options = {}) {
    const token = localStorage.getItem('authToken');
    
    const defaultOptions = {
        headers: {
            'Content-Type': 'application/json',
            ...(token && { 'Authorization': `Bearer ${token}` })
        }
    };
    
    const finalOptions = {
        ...defaultOptions,
        ...options,
        headers: {
            ...defaultOptions.headers,
            ...options.headers
        }
    };
    
    try {
        const response = await fetch(API_BASE_URL + url, finalOptions);
        
        if (response.status === 401) {
            // Token expired or invalid
            logout();
            window.location.href = '/Home/Login';
            return null;
        }
        
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        
        const contentType = response.headers.get('content-type');
        if (contentType && contentType.includes('application/json')) {
            return await response.json();
        }
        
        return response;
    } catch (error) {
        console.error('API request failed:', error);
        throw error;
    }
}

// ===== AUTHENTICATION FUNCTIONS =====
function getCurrentUser() {
    const userString = localStorage.getItem('currentUser');
    return userString ? JSON.parse(userString) : null;
}

function isAuthenticated() {
    return !!localStorage.getItem('authToken') && !!getCurrentUser();
}

function isAdmin() {
    const user = getCurrentUser();
    return user && user.roles && user.roles.includes('Admin');
}

function logout() {
    localStorage.removeItem('authToken');
    localStorage.removeItem('currentUser');
    localStorage.removeItem('refreshToken');
    currentUser = null;
    updateAuthUI();
    updateCartCount();
}

function updateAuthUI() {
    const userMenu = document.getElementById('userMenu');
    const loginMenu = document.getElementById('loginMenu');
    const userName = document.getElementById('userName');
    const adminLink = document.getElementById('adminLink');
    
    if (isAuthenticated()) {
        const user = getCurrentUser();
        userMenu.style.display = 'block';
        loginMenu.style.display = 'none';
        userName.textContent = user.firstName;
        
        if (isAdmin()) {
            adminLink.style.display = 'block';
            adminLink.href = '/Home/Admin';
        } else {
            adminLink.style.display = 'none';
        }
    } else {
        userMenu.style.display = 'none';
        loginMenu.style.display = 'block';
        adminLink.style.display = 'none';
    }
}

// ===== CART FUNCTIONS =====
async function addToCart(productId, quantity = 1) {
    if (!isAuthenticated()) {
        showToast('Please login to add items to cart', 'warning');
        setTimeout(() => {
            window.location.href = '/Home/Login';
        }, 2000);
        return;
    }
    
    try {
        showLoading();
        
        const response = await apiRequest('/cart/add', {
            method: 'POST',
            body: JSON.stringify({
                productId: productId,
                quantity: quantity
            })
        });
        
        if (response) {
            showToast('Product added to cart successfully!');
            updateCartCount();
        }
    } catch (error) {
        console.error('Error adding to cart:', error);
        showToast('Failed to add product to cart', 'danger');
    } finally {
        hideLoading();
    }
}

async function updateCartCount() {
    if (!isAuthenticated()) {
        document.getElementById('cartCount').textContent = '0';
        return;
    }
    
    try {
        const response = await apiRequest('/cart');
        if (response && response.items) {
            const totalItems = response.items.reduce((sum, item) => sum + item.quantity, 0);
            document.getElementById('cartCount').textContent = totalItems.toString();
            cartCount = totalItems;
        }
    } catch (error) {
        console.error('Error updating cart count:', error);
        document.getElementById('cartCount').textContent = '0';
    }
}

// ===== SEARCH FUNCTIONALITY =====
function initializeSearch() {
    const searchForm = document.getElementById('searchForm');
    const searchInput = document.getElementById('searchInput');
    
    if (searchForm && searchInput) {
        const debouncedSearch = debounce((query) => {
            if (query.trim()) {
                window.location.href = `/Home/Products?search=${encodeURIComponent(query)}`;
            }
        }, 300);
        
        searchForm.addEventListener('submit', (e) => {
            e.preventDefault();
            const query = searchInput.value.trim();
            if (query) {
                window.location.href = `/Home/Products?search=${encodeURIComponent(query)}`;
            }
        });
        
        searchInput.addEventListener('input', (e) => {
            const query = e.target.value;
            if (query.length >= 3) {
                debouncedSearch(query);
            }
        });
    }
}

// ===== CATEGORIES DROPDOWN =====
async function loadCategoriesDropdown() {
    try {
        const response = await apiRequest('/categories');
        if (response && response.length > 0) {
            const dropdown = document.getElementById('categoriesDropdown');
            if (dropdown) {
                dropdown.innerHTML = response.map(category => `
                    <li><a class="dropdown-item" href="/Home/Products?categoryId=${category.id}">
                        ${category.name}
                    </a></li>
                `).join('');
            }
        }
    } catch (error) {
        console.error('Error loading categories dropdown:', error);
    }
}

// ===== SMOOTH SCROLLING =====
function initializeSmoothScrolling() {
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            e.preventDefault();
            const target = document.querySelector(this.getAttribute('href'));
            if (target) {
                target.scrollIntoView({
                    behavior: 'smooth',
                    block: 'start'
                });
            }
        });
    });
}

// ===== FORM VALIDATION =====
function validateForm(formElement) {
    const inputs = formElement.querySelectorAll('input[required], select[required], textarea[required]');
    let isValid = true;
    
    inputs.forEach(input => {
        if (!input.value.trim()) {
            input.classList.add('is-invalid');
            isValid = false;
        } else {
            input.classList.remove('is-invalid');
            input.classList.add('is-valid');
        }
        
        // Email validation
        if (input.type === 'email' && input.value) {
            const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            if (!emailRegex.test(input.value)) {
                input.classList.add('is-invalid');
                input.classList.remove('is-valid');
                isValid = false;
            }
        }
        
        // Password confirmation
        if (input.name === 'confirmPassword') {
            const passwordInput = formElement.querySelector('input[name="password"]');
            if (passwordInput && input.value !== passwordInput.value) {
                input.classList.add('is-invalid');
                input.classList.remove('is-valid');
                isValid = false;
            }
        }
    });
    
    return isValid;
}

// ===== INITIALIZE ON DOM LOAD =====
document.addEventListener('DOMContentLoaded', function() {
    // Initialize authentication UI
    currentUser = getCurrentUser();
    updateAuthUI();
    updateCartCount();
    
    // Initialize search functionality
    initializeSearch();
    
    // Load categories dropdown
    loadCategoriesDropdown();
    
    // Initialize smooth scrolling
    initializeSmoothScrolling();
    
    // Logout button handler
    const logoutBtn = document.getElementById('logoutBtn');
    if (logoutBtn) {
        logoutBtn.addEventListener('click', function(e) {
            e.preventDefault();
            logout();
            showToast('Logged out successfully');
            setTimeout(() => {
                window.location.href = '/';
            }, 1000);
        });
    }
    
    // Initialize tooltips
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });
    
    // Auto-hide alerts
    const alerts = document.querySelectorAll('.alert[data-auto-dismiss]');
    alerts.forEach(alert => {
        setTimeout(() => {
            alert.style.opacity = '0';
            setTimeout(() => alert.remove(), 300);
        }, 5000);
    });
    
    // Add loading states to forms
    const forms = document.querySelectorAll('form');
    forms.forEach(form => {
        form.addEventListener('submit', function() {
            const submitBtn = form.querySelector('button[type="submit"]');
            if (submitBtn) {
                submitBtn.disabled = true;
                submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Loading...';
            }
        });
    });
    
    // Initialize page-specific functionality
    const currentPage = window.location.pathname.toLowerCase();
    
    if (currentPage.includes('/products')) {
        initializeProductsPage();
    } else if (currentPage.includes('/cart')) {
        initializeCartPage();
    } else if (currentPage.includes('/orders')) {
        initializeOrdersPage();
    }
});

// ===== PAGE-SPECIFIC INITIALIZATIONS =====
function initializeProductsPage() {
    // This will be implemented in products-specific JavaScript
    console.log('Products page initialized');
}

function initializeCartPage() {
    // This will be implemented in cart-specific JavaScript
    console.log('Cart page initialized');
}

function initializeOrdersPage() {
    // This will be implemented in orders-specific JavaScript
    console.log('Orders page initialized');
}

// ===== EXPORT FUNCTIONS FOR GLOBAL USE =====
window.ElectronicsStore = {
    showLoading,
    hideLoading,
    showToast,
    formatCurrency,
    formatDate,
    apiRequest,
    addToCart,
    updateCartCount,
    getCurrentUser,
    isAuthenticated,
    isAdmin,
    logout,
    validateForm
}; 