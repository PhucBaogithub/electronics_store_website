#!/bin/bash

# Electronics Store Docker Cleanup Script
# Usage: ./cleanup.sh [all|dev|prod|images|volumes]

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

# Stop and remove development containers
cleanup_dev() {
    print_status "Cleaning up development containers..."
    docker-compose -f docker-compose.dev.yml down --remove-orphans || true
    print_success "Development containers cleaned up."
}

# Stop and remove production containers
cleanup_prod() {
    print_status "Cleaning up production containers..."
    docker-compose down --remove-orphans || true
    print_success "Production containers cleaned up."
}

# Remove Docker images
cleanup_images() {
    print_status "Removing Electronics Store Docker images..."
    
    # Remove project images
    docker images | grep electronics-store | awk '{print $3}' | xargs -r docker rmi -f || true
    
    # Remove dangling images
    docker image prune -f || true
    
    print_success "Docker images cleaned up."
}

# Remove Docker volumes
cleanup_volumes() {
    print_warning "This will remove all data stored in Docker volumes!"
    read -p "Are you sure you want to continue? (y/N): " -n 1 -r
    echo
    
    if [[ $REPLY =~ ^[Yy]$ ]]; then
        print_status "Removing Docker volumes..."
        
        # Remove project volumes
        docker volume ls | grep electronics | awk '{print $2}' | xargs -r docker volume rm || true
        
        # Remove unused volumes
        docker volume prune -f || true
        
        print_success "Docker volumes cleaned up."
    else
        print_status "Volume cleanup cancelled."
    fi
}

# Complete cleanup
cleanup_all() {
    print_warning "This will remove all containers, images, and volumes!"
    read -p "Are you sure you want to continue? (y/N): " -n 1 -r
    echo
    
    if [[ $REPLY =~ ^[Yy]$ ]]; then
        cleanup_dev
        cleanup_prod
        cleanup_images
        cleanup_volumes
        
        # Clean up networks
        print_status "Cleaning up networks..."
        docker network ls | grep electronics | awk '{print $2}' | xargs -r docker network rm || true
        
        # System cleanup
        print_status "Running system cleanup..."
        docker system prune -f || true
        
        print_success "Complete cleanup finished."
    else
        print_status "Cleanup cancelled."
    fi
}

# Show usage
show_usage() {
    echo "Usage: $0 [all|dev|prod|images|volumes]"
    echo ""
    echo "Commands:"
    echo "  all      - Remove everything (containers, images, volumes, networks)"
    echo "  dev      - Remove development containers only"
    echo "  prod     - Remove production containers only"
    echo "  images   - Remove Docker images only"
    echo "  volumes  - Remove Docker volumes only (WARNING: This removes all data!)"
    echo ""
    echo "Examples:"
    echo "  $0 dev      # Clean up development containers"
    echo "  $0 prod     # Clean up production containers"
    echo "  $0 images   # Remove Docker images"
    echo "  $0 all      # Complete cleanup"
}

# Main script
main() {
    print_status "Electronics Store Docker Cleanup"
    print_status "================================="
    
    # Parse command line arguments
    case "${1:-}" in
        "all")
            cleanup_all
            ;;
        "dev")
            cleanup_dev
            ;;
        "prod")
            cleanup_prod
            ;;
        "images")
            cleanup_images
            ;;
        "volumes")
            cleanup_volumes
            ;;
        "")
            print_error "No cleanup type specified."
            show_usage
            exit 1
            ;;
        *)
            print_error "Invalid cleanup type: $1"
            show_usage
            exit 1
            ;;
    esac
}

# Run main function
main "$@"
