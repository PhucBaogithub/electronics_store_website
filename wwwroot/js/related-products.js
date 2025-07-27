/**
 * Related Products System for Electronics Store
 * Handles loading and displaying related products on product detail pages
 */

class RelatedProductsManager {
    constructor() {
        this.apiBaseUrl = '/api/Products';
        this.maxRetries = 3;
        this.retryDelay = 1000;
        this.init();
    }

    /**
     * Initialize related products system
     */
    init() {
        // Check if we're on a product detail page
        const productId = this.getProductIdFromUrl();
        if (productId) {
            this.loadRelatedProducts(productId);
        }
    }

    /**
     * Extract product ID from current URL
     */
    getProductIdFromUrl() {
        const path = window.location.pathname;
        const match = path.match(/\/Home\/ProductDetail\/(\d+)/);
        return match ? parseInt(match[1]) : null;
    }

    /**
     * Load related products for the given product ID
     */
    async loadRelatedProducts(productId, retryCount = 0) {
        try {
            this.showLoadingState();

            const response = await fetch(`${this.apiBaseUrl}/${productId}/related?count=6`);
            
            if (!response.ok) {
                throw new Error(`HTTP ${response.status}: ${response.statusText}`);
            }

            const relatedProducts = await response.json();
            
            if (relatedProducts && relatedProducts.length > 0) {
                this.renderRelatedProducts(relatedProducts);
            } else {
                this.showNoProductsMessage();
            }

        } catch (error) {
            console.error('Error loading related products:', error);
            
            // Retry logic
            if (retryCount < this.maxRetries) {
                console.log(`Retrying... Attempt ${retryCount + 1}/${this.maxRetries}`);
                setTimeout(() => {
                    this.loadRelatedProducts(productId, retryCount + 1);
                }, this.retryDelay * (retryCount + 1));
            } else {
                this.showErrorMessage();
            }
        }
    }

    /**
     * Show loading state
     */
    showLoadingState() {
        const container = this.getOrCreateContainer();
        container.innerHTML = `
            <div class="col-12">
                <div class="related-products-section">
                    <h3 class="mb-4">
                        <i class="fas fa-box-open me-2"></i>
                        Sản phẩm liên quan
                    </h3>
                    <div class="text-center py-4">
                        <div class="spinner-border text-primary" role="status">
                            <span class="visually-hidden">Đang tải...</span>
                        </div>
                        <p class="mt-2 text-muted">Đang tải sản phẩm liên quan...</p>
                    </div>
                </div>
            </div>
        `;
    }

    /**
     * Render related products
     */
    renderRelatedProducts(products) {
        const container = this.getOrCreateContainer();

        const productsHtml = products.map(product => this.createProductCard(product)).join('');

        container.innerHTML = `
            <div class="col-12">
                <div class="related-products-section">
                    <h3 class="mb-4">
                        <i class="fas fa-box-open me-2"></i>
                        Sản phẩm liên quan
                        <small class="text-muted">(${products.length} sản phẩm)</small>
                    </h3>
                    <div class="row g-3">
                        ${productsHtml}
                    </div>
                </div>
            </div>
        `;

        // Add click tracking
        this.addClickTracking();
    }

    /**
     * Create product card HTML
     */
    createProductCard(product) {
        const discountBadge = product.hasDiscount ? 
            `<span class="badge bg-danger position-absolute top-0 end-0 m-2">
                -${Math.round(((product.price - product.finalPrice) / product.price) * 100)}%
            </span>` : '';

        const stockStatus = product.isInStock ? 
            '<span class="badge bg-success">Còn hàng</span>' : 
            '<span class="badge bg-secondary">Hết hàng</span>';

        const priceHtml = product.hasDiscount ? 
            `<div class="price-section">
                <span class="text-decoration-line-through text-muted small">₫${this.formatPrice(product.price)}</span>
                <span class="fw-bold text-primary">₫${this.formatPrice(product.finalPrice)}</span>
            </div>` :
            `<div class="price-section">
                <span class="fw-bold text-primary">₫${this.formatPrice(product.finalPrice)}</span>
            </div>`;

        return `
            <div class="col-md-4 col-sm-6">
                <div class="card h-100 related-product-card" data-product-id="${product.id}">
                    <div class="position-relative">
                        <img src="${product.imageUrl || '/images/no-image.jpg'}" 
                             class="card-img-top" 
                             alt="${product.name}"
                             style="height: 200px; object-fit: cover;"
                             loading="lazy">
                        ${discountBadge}
                    </div>
                    <div class="card-body d-flex flex-column">
                        <h6 class="card-title text-truncate" title="${product.name}">
                            ${product.name}
                        </h6>
                        <p class="card-text text-muted small flex-grow-1" style="display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden;">
                            ${product.description || 'Không có mô tả'}
                        </p>
                        <div class="mt-auto">
                            ${priceHtml}
                            <div class="d-flex justify-content-between align-items-center mt-2">
                                ${stockStatus}
                                <small class="text-muted">${product.brand}</small>
                            </div>
                            <a href="/Home/ProductDetail/${product.id}" 
                               class="btn btn-outline-primary btn-sm mt-2 w-100 related-product-link">
                                <i class="fas fa-eye me-1"></i>
                                Xem chi tiết
                            </a>
                        </div>
                    </div>
                </div>
            </div>
        `;
    }

    /**
     * Show no products message
     */
    showNoProductsMessage() {
        const container = this.getOrCreateContainer();
        container.innerHTML = `
            <div class="col-12">
                <div class="related-products-section">
                    <h3 class="mb-4">
                        <i class="fas fa-box-open me-2"></i>
                        Sản phẩm liên quan
                    </h3>
                    <div class="text-center py-4">
                        <i class="fas fa-box-open fa-3x text-muted mb-3"></i>
                        <p class="text-muted">Không có sản phẩm liên quan nào được tìm thấy.</p>
                    </div>
                </div>
            </div>
        `;
    }

    /**
     * Show error message
     */
    showErrorMessage() {
        const container = this.getOrCreateContainer();
        container.innerHTML = `
            <div class="col-12">
                <div class="related-products-section">
                    <h3 class="mb-4">
                        <i class="fas fa-box-open me-2"></i>
                        Sản phẩm liên quan
                    </h3>
                    <div class="text-center py-4">
                        <i class="fas fa-exclamation-triangle fa-3x text-warning mb-3"></i>
                        <p class="text-muted">Không thể tải sản phẩm liên quan. Vui lòng thử lại sau.</p>
                        <button class="btn btn-outline-primary btn-sm" onclick="location.reload()">
                            <i class="fas fa-redo me-1"></i>
                            Thử lại
                        </button>
                    </div>
                </div>
            </div>
        `;
    }

    /**
     * Get or create container for related products
     */
    getOrCreateContainer() {
        let container = document.getElementById('related-products-container');

        if (!container) {
            // Create container if it doesn't exist (fallback)
            container = document.createElement('div');
            container.id = 'related-products-container';
            container.className = 'row mt-5';

            // Find the best place to insert the container - after reviews section
            const reviewsSection = document.querySelector('.row.mt-5');
            if (reviewsSection && reviewsSection.parentNode) {
                reviewsSection.parentNode.appendChild(container);
            } else {
                // Fallback: append to main container
                const mainContainer = document.querySelector('.container.py-4');
                if (mainContainer) {
                    mainContainer.appendChild(container);
                } else {
                    document.body.appendChild(container);
                }
            }
        }

        return container;
    }

    /**
     * Add click tracking for analytics
     */
    addClickTracking() {
        const links = document.querySelectorAll('.related-product-link');
        links.forEach(link => {
            link.addEventListener('click', (e) => {
                const productId = e.target.closest('.related-product-card').dataset.productId;
                
                // Google Analytics tracking (if available)
                if (typeof gtag !== 'undefined') {
                    gtag('event', 'click', {
                        'event_category': 'Related Products',
                        'event_label': `Product ID: ${productId}`,
                        'value': 1
                    });
                }
                
                console.log(`Related product clicked: ${productId}`);
            });
        });
    }

    /**
     * Format price with thousand separators
     */
    formatPrice(price) {
        return new Intl.NumberFormat('vi-VN').format(price);
    }

    /**
     * Refresh related products (useful for SPA navigation)
     */
    refresh() {
        const productId = this.getProductIdFromUrl();
        if (productId) {
            this.loadRelatedProducts(productId);
        }
    }
}

// Initialize when DOM is ready
document.addEventListener('DOMContentLoaded', function() {
    window.relatedProductsManager = new RelatedProductsManager();
});

// Expose for external use
window.RelatedProductsManager = RelatedProductsManager;
