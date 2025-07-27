#!/bin/bash

# Electronics Store Docker Deployment Script
# Usage: ./deploy.sh [dev|prod]

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Function to print colored output
print_status() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

print_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Check if Docker is installed
check_docker() {
    if ! command -v docker &> /dev/null; then
        print_error "Docker is not installed. Please install Docker first."
        exit 1
    fi
    
    if ! command -v docker-compose &> /dev/null; then
        print_error "Docker Compose is not installed. Please install Docker Compose first."
        exit 1
    fi
    
    print_success "Docker and Docker Compose are installed."
}

# Build and deploy for development
deploy_dev() {
    print_status "Deploying Electronics Store for Development..."
    
    # Stop existing containers
    print_status "Stopping existing development containers..."
    docker-compose -f docker-compose.dev.yml down || true
    
    # Build and start containers
    print_status "Building and starting development containers..."
    docker-compose -f docker-compose.dev.yml up --build -d
    
    # Wait for application to start
    print_status "Waiting for application to start..."
    sleep 10
    
    # Check if containers are running
    if docker-compose -f docker-compose.dev.yml ps | grep -q "Up"; then
        print_success "Development deployment completed successfully!"
        print_status "Application is running at: http://localhost:5221"
        print_status "To view logs: docker-compose -f docker-compose.dev.yml logs -f"
        print_status "To stop: docker-compose -f docker-compose.dev.yml down"
    else
        print_error "Deployment failed. Check logs with: docker-compose -f docker-compose.dev.yml logs"
        exit 1
    fi
}

# Build and deploy for production
deploy_prod() {
    print_status "Deploying Electronics Store for Production..."
    
    # Stop existing containers
    print_status "Stopping existing production containers..."
    docker-compose down || true
    
    # Build and start containers
    print_status "Building and starting production containers..."
    docker-compose up --build -d
    
    # Wait for application to start
    print_status "Waiting for application to start..."
    sleep 15
    
    # Check if containers are running
    if docker-compose ps | grep -q "Up"; then
        print_success "Production deployment completed successfully!"
        print_status "Application is running at: http://localhost:8080"
        print_status "Nginx proxy is running at: http://localhost:80"
        print_status "To view logs: docker-compose logs -f"
        print_status "To stop: docker-compose down"
    else
        print_error "Deployment failed. Check logs with: docker-compose logs"
        exit 1
    fi
}

# Show usage
show_usage() {
    echo "Usage: $0 [dev|prod]"
    echo ""
    echo "Commands:"
    echo "  dev   - Deploy for development (port 5221)"
    echo "  prod  - Deploy for production (port 8080 + nginx on port 80)"
    echo ""
    echo "Examples:"
    echo "  $0 dev    # Deploy for development"
    echo "  $0 prod   # Deploy for production"
}

# Main script
main() {
    print_status "Electronics Store Docker Deployment"
    print_status "===================================="
    
    # Check Docker installation
    check_docker
    
    # Parse command line arguments
    case "${1:-}" in
        "dev")
            deploy_dev
            ;;
        "prod")
            deploy_prod
            ;;
        "")
            print_error "No deployment type specified."
            show_usage
            exit 1
            ;;
        *)
            print_error "Invalid deployment type: $1"
            show_usage
            exit 1
            ;;
    esac
}

# Run main function
main "$@"
