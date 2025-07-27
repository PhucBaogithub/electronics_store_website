#!/bin/bash

# Electronics Store Database Connection Script
# Usage: ./connect_db.sh [local|docker]

set -e

# Colors for output
GREEN='\033[0;32m'
BLUE='\033[0;34m'
RED='\033[0;31m'
NC='\033[0m' # No Color

print_info() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

print_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Function to connect to local database
connect_local() {
    print_info "Connecting to local database..."
    
    # Find database file
    if [ -f "ElectronicsStore.db" ]; then
        DB_PATH="ElectronicsStore.db"
    elif [ -f "electronicsstore.db" ]; then
        DB_PATH="electronicsstore.db"
    else
        print_error "Database file not found in current directory"
        print_info "Please run the application first to create the database"
        exit 1
    fi
    
    print_success "Found database: $DB_PATH"
    
    # Check if DB Browser is installed
    if command -v "DB Browser for SQLite" &> /dev/null; then
        print_info "Opening with DB Browser for SQLite..."
        open -a "DB Browser for SQLite" "$DB_PATH"
    elif [ -d "/Applications/DB Browser for SQLite.app" ]; then
        print_info "Opening with DB Browser for SQLite..."
        open -a "/Applications/DB Browser for SQLite.app" "$DB_PATH"
    else
        print_info "DB Browser not found. Opening with SQLite command line..."
        sqlite3 "$DB_PATH"
    fi
}

# Function to connect to Docker database
connect_docker() {
    print_info "Connecting to Docker database..."
    
    # Check if container is running
    if ! docker ps | grep -q "electronics-store"; then
        print_error "Electronics Store container is not running"
        print_info "Please start the container first: ./deploy.sh dev"
        exit 1
    fi
    
    # Copy database from container
    print_info "Copying database from Docker container..."
    docker cp electronics-store-dev:/app/Data/ElectronicsStore.db ./ElectronicsStore_docker.db
    
    if [ $? -eq 0 ]; then
        print_success "Database copied successfully"
        
        # Open with GUI tool
        if command -v "DB Browser for SQLite" &> /dev/null; then
            print_info "Opening with DB Browser for SQLite..."
            open -a "DB Browser for SQLite" "ElectronicsStore_docker.db"
        elif [ -d "/Applications/DB Browser for SQLite.app" ]; then
            print_info "Opening with DB Browser for SQLite..."
            open -a "/Applications/DB Browser for SQLite.app" "ElectronicsStore_docker.db"
        else
            print_info "DB Browser not found. Opening with SQLite command line..."
            sqlite3 "ElectronicsStore_docker.db"
        fi
    else
        print_error "Failed to copy database from container"
        exit 1
    fi
}

# Function to show database info
show_info() {
    print_info "Database Information:"
    echo "===================="
    
    if [ -f "ElectronicsStore.db" ]; then
        echo "Local Database: ElectronicsStore.db"
        echo "Size: $(ls -lh ElectronicsStore.db | awk '{print $5}')"
        echo "Modified: $(stat -f %Sm ElectronicsStore.db)"
    fi
    
    if docker ps | grep -q "electronics-store"; then
        echo "Docker Container: Running"
        echo "Container Name: electronics-store-dev"
    else
        echo "Docker Container: Not running"
    fi
    
    echo ""
    echo "Available GUI Tools:"
    if [ -d "/Applications/DB Browser for SQLite.app" ]; then
        echo "✓ DB Browser for SQLite"
    fi
    if [ -d "/Applications/TablePlus.app" ]; then
        echo "✓ TablePlus"
    fi
    if [ -d "/Applications/SQLiteStudio.app" ]; then
        echo "✓ SQLiteStudio"
    fi
    if [ -d "/Applications/DBeaver.app" ]; then
        echo "✓ DBeaver"
    fi
}

# Main function
main() {
    case "${1:-}" in
        "local")
            connect_local
            ;;
        "docker")
            connect_docker
            ;;
        "info")
            show_info
            ;;
        "")
            print_error "Please specify connection type"
            echo "Usage: $0 [local|docker|info]"
            echo ""
            echo "Examples:"
            echo "  $0 local   # Connect to local database"
            echo "  $0 docker  # Connect to Docker database"
            echo "  $0 info    # Show database information"
            ;;
        *)
            print_error "Invalid option: $1"
            echo "Usage: $0 [local|docker|info]"
            ;;
    esac
}

main "$@"
