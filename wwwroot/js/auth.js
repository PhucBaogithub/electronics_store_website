// ===== AUTHENTICATION HANDLERS =====

// Login functionality
async function handleLogin(event) {
    event.preventDefault();
    
    const form = event.target;
    const formData = new FormData(form);
    
    const loginData = {
        email: formData.get('email'),
        password: formData.get('password'),
        rememberMe: formData.get('rememberMe') === 'on'
    };
    
    try {
        showLoading();
        
        const response = await fetch('/api/auth/login', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(loginData)
        });
        
        if (response.ok) {
            const data = await response.json();
            
            // Store authentication data
            localStorage.setItem('authToken', data.token);
            localStorage.setItem('currentUser', JSON.stringify(data.user));
            localStorage.setItem('refreshToken', data.refreshToken);
            
            showToast('Login successful! Redirecting...', 'success');
            
            // Update UI
            updateAuthUI();
            updateCartCount();
            
            // Redirect after short delay
            setTimeout(() => {
                const returnUrl = new URLSearchParams(window.location.search).get('returnUrl');
                window.location.href = returnUrl || '/';
            }, 1500);
            
        } else {
            const errorData = await response.text();
            showToast(errorData || 'Login failed. Please check your credentials.', 'danger');
        }
    } catch (error) {
        console.error('Login error:', error);
        showToast('An error occurred during login. Please try again.', 'danger');
    } finally {
        hideLoading();
    }
}

// Register functionality
async function handleRegister(event) {
    event.preventDefault();
    
    const form = event.target;
    const formData = new FormData(form);
    
    const registerData = {
        firstName: formData.get('firstName'),
        lastName: formData.get('lastName'),
        email: formData.get('email'),
        password: formData.get('password'),
        confirmPassword: formData.get('confirmPassword'),
        phoneNumber: formData.get('phoneNumber')
    };
    
    // Client-side validation
    if (registerData.password !== registerData.confirmPassword) {
        showToast('Passwords do not match!', 'danger');
        return;
    }
    
    if (registerData.password.length < 6) {
        showToast('Password must be at least 6 characters long!', 'danger');
        return;
    }
    
    try {
        showLoading();
        
        const response = await fetch('/api/auth/register', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(registerData)
        });
        
        if (response.ok) {
            const data = await response.json();
            
            // Store authentication data
            localStorage.setItem('authToken', data.token);
            localStorage.setItem('currentUser', JSON.stringify(data.user));
            localStorage.setItem('refreshToken', data.refreshToken);
            
            showToast('Registration successful! Welcome to Electronics Store!', 'success');
            
            // Update UI
            updateAuthUI();
            updateCartCount();
            
            // Redirect after short delay
            setTimeout(() => {
                window.location.href = '/';
            }, 1500);
            
        } else {
            const errorData = await response.json();
            let errorMessage = 'Registration failed. Please try again.';
            
            if (errorData.errors) {
                // Handle validation errors
                const errors = Object.values(errorData.errors).flat();
                errorMessage = errors.join(' ');
            } else if (errorData.message) {
                errorMessage = errorData.message;
            }
            
            showToast(errorMessage, 'danger');
        }
    } catch (error) {
        console.error('Registration error:', error);
        showToast('An error occurred during registration. Please try again.', 'danger');
    } finally {
        hideLoading();
    }
}

// Password strength indicator
function updatePasswordStrength(password) {
    const strengthIndicator = document.getElementById('passwordStrength');
    const strengthText = document.getElementById('passwordStrengthText');
    
    if (!strengthIndicator || !strengthText) return;
    
    let strength = 0;
    let feedback = [];
    
    // Length check
    if (password.length >= 8) {
        strength += 1;
    } else {
        feedback.push('At least 8 characters');
    }
    
    // Uppercase check
    if (/[A-Z]/.test(password)) {
        strength += 1;
    } else {
        feedback.push('One uppercase letter');
    }
    
    // Lowercase check
    if (/[a-z]/.test(password)) {
        strength += 1;
    } else {
        feedback.push('One lowercase letter');
    }
    
    // Number check
    if (/\d/.test(password)) {
        strength += 1;
    } else {
        feedback.push('One number');
    }
    
    // Special character check
    if (/[!@#$%^&*(),.?":{}|<>]/.test(password)) {
        strength += 1;
    } else {
        feedback.push('One special character');
    }
    
    // Update UI
    const strengthLevels = ['Very Weak', 'Weak', 'Fair', 'Good', 'Strong'];
    const strengthColors = ['danger', 'warning', 'info', 'success', 'success'];
    
    const level = Math.min(strength, 4);
    strengthIndicator.className = `progress-bar bg-${strengthColors[level]}`;
    strengthIndicator.style.width = `${(level + 1) * 20}%`;
    strengthText.textContent = strengthLevels[level];
    
    if (feedback.length > 0 && password.length > 0) {
        strengthText.textContent += ` (Need: ${feedback.join(', ')})`;
    }
}

// Toggle password visibility
function togglePasswordVisibility(inputId, buttonId) {
    const input = document.getElementById(inputId);
    const button = document.getElementById(buttonId);
    const icon = button.querySelector('i');
    
    if (input.type === 'password') {
        input.type = 'text';
        icon.className = 'fas fa-eye-slash';
    } else {
        input.type = 'password';
        icon.className = 'fas fa-eye';
    }
}

// Initialize authentication pages
function initializeAuthPages() {
    // Login form
    const loginForm = document.getElementById('loginForm');
    if (loginForm) {
        loginForm.addEventListener('submit', handleLogin);
        
        // Add real-time validation
        const emailInput = loginForm.querySelector('input[name="email"]');
        const passwordInput = loginForm.querySelector('input[name="password"]');
        
        if (emailInput) {
            emailInput.addEventListener('blur', function() {
                const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
                if (this.value && !emailRegex.test(this.value)) {
                    this.classList.add('is-invalid');
                } else {
                    this.classList.remove('is-invalid');
                }
            });
        }
        
        if (passwordInput) {
            passwordInput.addEventListener('input', function() {
                if (this.value.length < 6 && this.value.length > 0) {
                    this.classList.add('is-invalid');
                } else {
                    this.classList.remove('is-invalid');
                }
            });
        }
    }
    
    // Register form
    const registerForm = document.getElementById('registerForm');
    if (registerForm) {
        registerForm.addEventListener('submit', handleRegister);
        
        // Password strength indicator
        const passwordInput = registerForm.querySelector('input[name="password"]');
        if (passwordInput) {
            passwordInput.addEventListener('input', function() {
                updatePasswordStrength(this.value);
            });
        }
        
        // Confirm password validation
        const confirmPasswordInput = registerForm.querySelector('input[name="confirmPassword"]');
        if (confirmPasswordInput && passwordInput) {
            confirmPasswordInput.addEventListener('input', function() {
                if (this.value && this.value !== passwordInput.value) {
                    this.classList.add('is-invalid');
                    this.nextElementSibling.textContent = 'Passwords do not match';
                } else {
                    this.classList.remove('is-invalid');
                }
            });
        }
        
        // Real-time validation for other fields
        const requiredFields = registerForm.querySelectorAll('input[required]');
        requiredFields.forEach(field => {
            field.addEventListener('blur', function() {
                if (!this.value.trim()) {
                    this.classList.add('is-invalid');
                } else {
                    this.classList.remove('is-invalid');
                }
            });
        });
    }
    
    // Password toggle buttons
    const passwordToggles = document.querySelectorAll('[data-password-toggle]');
    passwordToggles.forEach(button => {
        button.addEventListener('click', function() {
            const targetId = this.getAttribute('data-password-toggle');
            togglePasswordVisibility(targetId, this.id);
        });
    });
    
    // Check if user is already logged in
    if (isAuthenticated()) {
        const currentPath = window.location.pathname.toLowerCase();
        if (currentPath.includes('/login') || currentPath.includes('/register')) {
            showToast('You are already logged in. Redirecting...', 'info');
            setTimeout(() => {
                window.location.href = '/';
            }, 2000);
        }
    }
}

// Social login (placeholder for future implementation)
function handleSocialLogin(provider) {
    showToast(`${provider} login will be available soon!`, 'info');
}

// Forgot password (placeholder for future implementation)
function handleForgotPassword() {
    showToast('Password reset functionality will be available soon!', 'info');
}

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', initializeAuthPages); 