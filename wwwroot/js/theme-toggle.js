/**
 * Theme Toggle System for Electronics Store
 * Handles dark/light theme switching with localStorage persistence
 */

class ThemeManager {
    constructor() {
        this.themeKey = 'electronics-store-theme';
        this.currentTheme = this.getStoredTheme() || this.getSystemPreference() || 'light';
        this.init();
    }

    /**
     * Initialize theme system
     */
    init() {
        this.applyTheme(this.currentTheme);
        this.setupToggleButton();
        this.bindEvents();
        
        // Listen for system theme changes
        if (window.matchMedia) {
            window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', (e) => {
                if (!this.getStoredTheme()) {
                    this.setTheme(e.matches ? 'dark' : 'light');
                }
            });
        }
    }

    /**
     * Get theme from localStorage
     */
    getStoredTheme() {
        try {
            return localStorage.getItem(this.themeKey);
        } catch (e) {
            console.warn('localStorage not available, using default theme');
            return null;
        }
    }

    /**
     * Get system theme preference
     */
    getSystemPreference() {
        if (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches) {
            return 'dark';
        }
        return 'light'; // Default to light theme
    }

    /**
     * Store theme in localStorage
     */
    setStoredTheme(theme) {
        try {
            localStorage.setItem(this.themeKey, theme);
        } catch (e) {
            console.warn('Could not save theme preference');
        }
    }

    /**
     * Apply theme to document
     */
    applyTheme(theme) {
        // Ensure theme is valid
        if (theme !== 'light' && theme !== 'dark') {
            theme = 'light';
        }

        document.documentElement.setAttribute('data-theme', theme);
        this.currentTheme = theme;

        // Update body class for additional styling if needed
        document.body.classList.remove('theme-light', 'theme-dark');
        document.body.classList.add(`theme-${theme}`);

        // Update toggle button state
        const toggleInput = document.getElementById('theme-toggle-input');
        if (toggleInput) {
            toggleInput.checked = theme === 'dark';
        }

        // Update meta theme-color for mobile browsers
        let metaThemeColor = document.querySelector('meta[name="theme-color"]');
        if (!metaThemeColor) {
            metaThemeColor = document.createElement('meta');
            metaThemeColor.name = 'theme-color';
            document.head.appendChild(metaThemeColor);
        }
        metaThemeColor.content = theme === 'dark' ? '#121212' : '#ffffff';

        // Dispatch custom event for other components
        window.dispatchEvent(new CustomEvent('themeChanged', {
            detail: { theme: theme }
        }));

        console.log(`Theme applied: ${theme}`);
    }

    /**
     * Set theme and persist to storage
     */
    setTheme(theme) {
        this.applyTheme(theme);
        this.setStoredTheme(theme);
    }

    /**
     * Toggle between light and dark themes
     */
    toggleTheme() {
        const newTheme = this.currentTheme === 'light' ? 'dark' : 'light';
        this.setTheme(newTheme);
        
        // Analytics tracking (if available)
        if (typeof gtag !== 'undefined') {
            gtag('event', 'theme_toggle', {
                'theme': newTheme
            });
        }
    }

    /**
     * Setup toggle button in navbar
     */
    setupToggleButton() {
        const navbar = document.querySelector('.navbar-nav');
        if (!navbar) return;

        // Check if toggle already exists
        if (document.getElementById('theme-toggle-container')) return;

        const toggleContainer = document.createElement('li');
        toggleContainer.className = 'nav-item d-flex align-items-center';
        toggleContainer.id = 'theme-toggle-container';

        toggleContainer.innerHTML = `
            <label class="theme-toggle" for="theme-toggle-input" title="Toggle dark/light theme">
                <input type="checkbox" id="theme-toggle-input" ${this.currentTheme === 'dark' ? 'checked' : ''}>
                <span class="theme-slider">
                    <i class="fas fa-sun theme-icon sun-icon"></i>
                    <i class="fas fa-moon theme-icon moon-icon"></i>
                </span>
            </label>
        `;

        navbar.appendChild(toggleContainer);
    }

    /**
     * Bind event listeners
     */
    bindEvents() {
        // Toggle button click
        document.addEventListener('change', (e) => {
            if (e.target && e.target.id === 'theme-toggle-input') {
                this.toggleTheme();
            }
        });

        // Keyboard shortcut (Ctrl/Cmd + Shift + T)
        document.addEventListener('keydown', (e) => {
            if ((e.ctrlKey || e.metaKey) && e.shiftKey && e.key === 'T') {
                e.preventDefault();
                this.toggleTheme();
            }
        });
    }

    /**
     * Get current theme
     */
    getCurrentTheme() {
        return this.currentTheme;
    }

    /**
     * Check if dark theme is active
     */
    isDarkTheme() {
        return this.currentTheme === 'dark';
    }
}

// Initialize theme manager when DOM is ready
document.addEventListener('DOMContentLoaded', function() {
    window.themeManager = new ThemeManager();
});

// Expose theme manager globally for other scripts
window.ThemeManager = ThemeManager;

// Utility functions for other components
window.getTheme = function() {
    return window.themeManager ? window.themeManager.getCurrentTheme() : 'light';
};

window.isDarkTheme = function() {
    return window.themeManager ? window.themeManager.isDarkTheme() : false;
};

window.toggleTheme = function() {
    if (window.themeManager) {
        window.themeManager.toggleTheme();
    }
};

// CSS-in-JS for dynamic theme adjustments
window.getThemeColors = function() {
    const theme = window.getTheme();
    
    if (theme === 'dark') {
        return {
            primary: '#121212',
            secondary: '#1e1e1e',
            text: '#ffffff',
            textSecondary: '#b3b3b3',
            border: '#404040'
        };
    } else {
        return {
            primary: '#ffffff',
            secondary: '#f8f9fa',
            text: '#212529',
            textSecondary: '#6c757d',
            border: '#dee2e6'
        };
    }
};

// Export for module systems
if (typeof module !== 'undefined' && module.exports) {
    module.exports = ThemeManager;
}
