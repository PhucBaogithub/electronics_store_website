// Chatbot JavaScript Functionality
class Chatbot {
    constructor() {
        this.conversationId = null;
        this.isOpen = false;
        this.isMinimized = false;
        this.isTyping = false;
        this.messageQueue = [];
        this.init();
    }

    init() {
        // Load conversation ID from localStorage
        this.conversationId = localStorage.getItem('chatbot_conversation_id');
        
        // Load chat state
        const chatState = localStorage.getItem('chatbot_state');
        if (chatState) {
            const state = JSON.parse(chatState);
            this.isOpen = state.isOpen || false;
            this.isMinimized = state.isMinimized || false;
        }

        // Restore chat window state
        if (this.isOpen) {
            this.showChatWindow();
            if (this.isMinimized) {
                this.minimizeChatbot();
            }
        }

        // Load chat history if conversation exists
        if (this.conversationId) {
            this.loadChatHistory();
        }

        // Auto-show welcome message after 3 seconds if first visit
        if (!localStorage.getItem('chatbot_welcomed')) {
            setTimeout(() => {
                this.showWelcomeNotification();
            }, 3000);
        }
    }

    showWelcomeNotification() {
        const notification = document.getElementById('chatbot-notification');
        if (notification) {
            notification.textContent = '1';
            notification.style.display = 'flex';
            localStorage.setItem('chatbot_welcomed', 'true');
        }
    }

    showWelcomeMessage() {
        const welcomeMessage = {
            message: "👋 Xin chào! Tôi là trợ lý ảo thông minh của Electronics Store.\n\n🤖 Tôi có thể hỗ trợ bạn:\n• 🔍 Tìm kiếm sản phẩm điện tử\n• 📦 Kiểm tra đơn hàng và giao hàng\n• 🛠️ Hỗ trợ kỹ thuật và sửa chữa\n• 🛡️ Thông tin bảo hành\n• 💰 Tư vấn giá cả và khuyến mãi\n• 👨‍💼 Kết nối với nhân viên tư vấn\n\n💬 Hãy cho tôi biết bạn cần hỗ trợ gì nhé!",
            isFromUser: false,
            timestamp: new Date(),
            messageType: 'text'
        };
        this.addMessage(welcomeMessage);
    }

    toggleChatbot() {
        if (this.isOpen) {
            this.closeChatbot();
        } else {
            this.openChatbot();
        }
    }

    openChatbot() {
        this.isOpen = true;
        this.isMinimized = false;
        this.showChatWindow();
        this.hideNotification();
        this.saveChatState();
        this.focusInput();

        // Hiển thị tin nhắn chào mừng nếu chưa có tin nhắn nào
        if (this.messages.length === 0) {
            this.showWelcomeMessage();
        }
    }

    closeChatbot() {
        this.isOpen = false;
        this.isMinimized = false;
        this.hideChatWindow();
        this.saveChatState();
    }

    minimizeChatbot() {
        this.isMinimized = true;
        this.hideChatWindow();
        this.saveChatState();
    }

    showChatWindow() {
        const chatWindow = document.getElementById('chatbot-window');
        if (chatWindow) {
            chatWindow.style.display = 'flex';
            this.scrollToBottom();
        }
    }

    hideChatWindow() {
        const chatWindow = document.getElementById('chatbot-window');
        if (chatWindow) {
            chatWindow.style.display = 'none';
        }
    }

    hideNotification() {
        const notification = document.getElementById('chatbot-notification');
        if (notification) {
            notification.style.display = 'none';
        }
    }

    focusInput() {
        setTimeout(() => {
            const input = document.getElementById('chatbot-message-input');
            if (input) {
                input.focus();
            }
        }, 100);
    }

    saveChatState() {
        const state = {
            isOpen: this.isOpen,
            isMinimized: this.isMinimized
        };
        localStorage.setItem('chatbot_state', JSON.stringify(state));
    }

    async sendMessage(message = null) {
        const input = document.getElementById('chatbot-message-input');
        const messageText = message || input?.value?.trim();

        if (!messageText) return;

        // Clear input
        if (input && !message) {
            input.value = '';
        }

        // Show user message immediately
        this.addMessage(messageText, true);
        
        // Show typing indicator
        this.showTyping();

        try {
            const response = await fetch('/api/chat/send', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                credentials: 'include',
                body: JSON.stringify({
                    message: messageText,
                    conversationId: this.conversationId
                })
            });

            const data = await response.json();

            if (data.success) {
                // Update conversation ID
                this.conversationId = data.conversationId;
                localStorage.setItem('chatbot_conversation_id', this.conversationId);

                // Add bot response
                setTimeout(() => {
                    this.hideTyping();
                    this.addBotMessage(data.botResponse);
                }, 1000); // Simulate typing delay
            } else {
                this.hideTyping();
                this.addMessage('Xin lỗi, đã xảy ra lỗi. Vui lòng thử lại.', false);
            }
        } catch (error) {
            console.error('Error sending message:', error);
            this.hideTyping();
            this.addMessage('Không thể kết nối đến server. Vui lòng kiểm tra kết nối mạng.', false);
        }
    }

    addMessage(message, isUser = false) {
        const messagesContainer = document.getElementById('chatbot-messages');
        if (!messagesContainer) return;

        const messageElement = document.createElement('div');
        messageElement.className = `chatbot-message ${isUser ? 'user-message' : 'bot-message'}`;

        const avatar = document.createElement('div');
        avatar.className = 'message-avatar';
        avatar.innerHTML = isUser ? '<i class="fas fa-user"></i>' : '<i class="fas fa-robot"></i>';

        const content = document.createElement('div');
        content.className = 'message-content';

        const bubble = document.createElement('div');
        bubble.className = 'message-bubble';
        bubble.innerHTML = this.formatMessage(message);

        const time = document.createElement('div');
        time.className = 'message-time';
        time.innerHTML = '<small class="text-muted">Vừa xong</small>';

        content.appendChild(bubble);
        content.appendChild(time);

        messageElement.appendChild(avatar);
        messageElement.appendChild(content);

        messagesContainer.appendChild(messageElement);
        this.scrollToBottom();
    }

    addBotMessage(botResponse) {
        if (botResponse.messageType === 'ProductRecommendation' && botResponse.productRecommendation) {
            this.addProductRecommendation(botResponse);
        } else {
            this.addMessage(botResponse.message, false);
        }
    }

    addProductRecommendation(botResponse) {
        const messagesContainer = document.getElementById('chatbot-messages');
        if (!messagesContainer) return;

        // Add text message first
        this.addMessage(botResponse.message, false);

        // Add product card
        const messageElement = document.createElement('div');
        messageElement.className = 'chatbot-message bot-message';

        const avatar = document.createElement('div');
        avatar.className = 'message-avatar';
        avatar.innerHTML = '<i class="fas fa-robot"></i>';

        const content = document.createElement('div');
        content.className = 'message-content';

        const product = botResponse.productRecommendation;
        const productCard = document.createElement('div');
        productCard.className = 'message-bubble product-recommendation';
        productCard.innerHTML = `
            <div class="product-card-mini">
                <img src="${product.imageUrl || 'https://via.placeholder.com/80x80/007bff/ffffff?text=No+Image'}" 
                     alt="${product.name}" class="product-image">
                <div class="product-info">
                    <h6 class="product-name">${product.name}</h6>
                    <p class="product-brand">${product.brand || ''}</p>
                    <div class="product-price">
                        ${product.discountPrice ? 
                            `<span class="price-discount">${this.formatPrice(product.discountPrice)}</span>
                             <span class="price-original">${this.formatPrice(product.price)}</span>` :
                            `<span class="price">${this.formatPrice(product.price)}</span>`
                        }
                    </div>
                    <div class="product-actions">
                        <a href="${product.productUrl}" class="btn btn-sm btn-primary" target="_blank">
                            <i class="fas fa-eye me-1"></i>Xem chi tiết
                        </a>
                        ${product.isInStock ? 
                            `<button class="btn btn-sm btn-success" onclick="chatbot.addToCartFromChat(${product.productId})">
                                <i class="fas fa-cart-plus me-1"></i>Thêm vào giỏ
                             </button>` :
                            `<button class="btn btn-sm btn-secondary" disabled>
                                <i class="fas fa-ban me-1"></i>Hết hàng
                             </button>`
                        }
                    </div>
                </div>
            </div>
        `;

        content.appendChild(productCard);
        messageElement.appendChild(avatar);
        messageElement.appendChild(content);

        messagesContainer.appendChild(messageElement);
        this.scrollToBottom();
    }

    async addToCartFromChat(productId) {
        try {
            const formData = new FormData();
            formData.append('productId', productId);
            formData.append('quantity', 1);

            const response = await fetch('/Cart/AddToCart', {
                method: 'POST',
                body: formData,
                credentials: 'include'
            });

            if (response.ok) {
                this.addMessage('✅ Đã thêm sản phẩm vào giỏ hàng thành công!', false);
                // Update cart count if function exists
                if (window.updateCartCount) {
                    window.updateCartCount();
                }
            } else if (response.redirected && response.url.includes('/Account/Login')) {
                this.addMessage('Bạn cần đăng nhập để thêm sản phẩm vào giỏ hàng. <a href="/Account/Login" target="_blank">Đăng nhập ngay</a>', false);
            } else {
                this.addMessage('❌ Không thể thêm sản phẩm vào giỏ hàng. Vui lòng thử lại.', false);
            }
        } catch (error) {
            console.error('Error adding to cart:', error);
            this.addMessage('❌ Đã xảy ra lỗi khi thêm vào giỏ hàng.', false);
        }
    }

    formatMessage(message) {
        // Convert line breaks to <br>
        return message.replace(/\n/g, '<br>');
    }

    formatPrice(price) {
        return new Intl.NumberFormat('vi-VN', {
            style: 'currency',
            currency: 'VND'
        }).format(price);
    }

    showTyping() {
        this.isTyping = true;
        const typingElement = document.getElementById('chatbot-typing');
        if (typingElement) {
            typingElement.style.display = 'flex';
            this.scrollToBottom();
        }
    }

    hideTyping() {
        this.isTyping = false;
        const typingElement = document.getElementById('chatbot-typing');
        if (typingElement) {
            typingElement.style.display = 'none';
        }
    }

    scrollToBottom() {
        setTimeout(() => {
            const messagesContainer = document.getElementById('chatbot-messages');
            if (messagesContainer) {
                messagesContainer.scrollTop = messagesContainer.scrollHeight;
            }
        }, 100);
    }

    async loadChatHistory() {
        try {
            const response = await fetch(`/api/chat/history/${this.conversationId}`, {
                credentials: 'include'
            });

            if (response.ok) {
                const data = await response.json();
                const messagesContainer = document.getElementById('chatbot-messages');
                
                // Clear existing messages except welcome message
                if (messagesContainer) {
                    const welcomeMessage = messagesContainer.querySelector('.chatbot-message');
                    messagesContainer.innerHTML = '';
                    if (welcomeMessage) {
                        messagesContainer.appendChild(welcomeMessage);
                    }
                }

                // Add historical messages
                data.messages.forEach(message => {
                    if (message.messageType === 'ProductRecommendation') {
                        this.addBotMessage(message);
                    } else {
                        this.addMessage(message.message, message.isFromUser);
                    }
                });
            }
        } catch (error) {
            console.error('Error loading chat history:', error);
        }
    }
}

// Global chatbot instance
let chatbot;

// Global functions for HTML onclick events
function toggleChatbot() {
    chatbot.toggleChatbot();
}

function closeChatbot() {
    chatbot.closeChatbot();
}

function minimizeChatbot() {
    chatbot.minimizeChatbot();
}

function sendMessage() {
    chatbot.sendMessage();
}

function sendQuickReply(message) {
    chatbot.sendMessage(message);
}

function handleChatInputKeypress(event) {
    if (event.key === 'Enter' && !event.shiftKey) {
        event.preventDefault();
        chatbot.sendMessage();
    }
}

// Global functions for HTML onclick handlers
function toggleChatbot() {
    if (window.chatbot) {
        window.chatbot.toggleChatbot();
    }
}

function minimizeChatbot() {
    if (window.chatbot) {
        window.chatbot.minimizeChatbot();
    }
}

function closeChatbot() {
    if (window.chatbot) {
        window.chatbot.closeChatbot();
    }
}

function sendMessage() {
    if (window.chatbot) {
        window.chatbot.sendMessage();
    }
}

function sendQuickReply(message) {
    if (window.chatbot) {
        const input = document.getElementById('chatbot-message-input');
        if (input) {
            input.value = message;
            window.chatbot.sendMessage();
        }
    }
}

function handleChatInputKeypress(event) {
    if (event.key === 'Enter' && !event.shiftKey) {
        event.preventDefault();
        sendMessage();
    }
}

// Initialize chatbot when DOM is loaded
document.addEventListener('DOMContentLoaded', function() {
    window.chatbot = new Chatbot();
});
