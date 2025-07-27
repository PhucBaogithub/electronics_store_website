# Electronics Store - Docker Deployment Guide

This guide explains how to deploy the Electronics Store application using Docker and Docker Compose.

## Prerequisites

- Docker (version 20.10 or later)
- Docker Compose (version 2.0 or later)
- Git (to clone the repository)

### Installing Docker

#### Windows/Mac:
- Download and install Docker Desktop from [docker.com](https://www.docker.com/products/docker-desktop)

#### Linux (Ubuntu/Debian):
```bash
# Update package index
sudo apt-get update

# Install Docker
curl -fsSL https://get.docker.com -o get-docker.sh
sudo sh get-docker.sh

# Install Docker Compose
sudo apt-get install docker-compose-plugin

# Add user to docker group
sudo usermod -aG docker $USER
```

## Quick Start

### 1. Clone the Repository
```bash
git clone <your-repository-url>
cd ElectronicsStore
```

### 2. Make Scripts Executable
```bash
chmod +x deploy.sh cleanup.sh
```

### 3. Deploy for Development
```bash
./deploy.sh dev
```

### 4. Deploy for Production
```bash
./deploy.sh prod
```

## Deployment Options

### Development Deployment
- **Port**: 5221
- **Environment**: Development
- **Features**: Hot reload support, development logging
- **Command**: `./deploy.sh dev`
- **Access**: http://localhost:5221

### Production Deployment
- **Port**: 8080 (app), 80 (nginx)
- **Environment**: Production
- **Features**: Nginx reverse proxy, rate limiting, SSL ready
- **Command**: `./deploy.sh prod`
- **Access**: http://localhost:80 (nginx) or http://localhost:8080 (direct)

## Manual Docker Commands

### Development
```bash
# Build and start development containers
docker-compose -f docker-compose.dev.yml up --build -d

# View logs
docker-compose -f docker-compose.dev.yml logs -f

# Stop containers
docker-compose -f docker-compose.dev.yml down
```

### Production
```bash
# Build and start production containers
docker-compose up --build -d

# View logs
docker-compose logs -f

# Stop containers
docker-compose down
```

## Configuration

### Environment Variables
The application uses the following environment variables:

- `ASPNETCORE_ENVIRONMENT`: Development/Production
- `ASPNETCORE_URLS`: Application URLs
- `ConnectionStrings__DefaultConnection`: Database connection string

### Database
- **Type**: SQLite
- **Location**: `/app/Data/ElectronicsStore.db` (inside container)
- **Persistence**: Data is stored in Docker volumes

### Volumes
- `electronics-data`: Production database storage
- `electronics-dev-data`: Development database storage

## Monitoring and Maintenance

### View Application Logs
```bash
# Development
docker-compose -f docker-compose.dev.yml logs -f electronics-store-dev

# Production
docker-compose logs -f electronics-store
```

### View Nginx Logs (Production)
```bash
docker-compose logs -f nginx
```

### Access Container Shell
```bash
# Development
docker exec -it electronics-store-dev /bin/bash

# Production
docker exec -it electronics-store-app /bin/bash
```

### Database Backup
```bash
# Create backup
docker exec electronics-store-app sqlite3 /app/Data/ElectronicsStore.db ".backup /app/Data/backup.db"

# Copy backup to host
docker cp electronics-store-app:/app/Data/backup.db ./backup.db
```

## Cleanup

### Clean Up Development
```bash
./cleanup.sh dev
```

### Clean Up Production
```bash
./cleanup.sh prod
```

### Complete Cleanup (removes all data)
```bash
./cleanup.sh all
```

## Troubleshooting

### Common Issues

1. **Port Already in Use**
   ```bash
   # Check what's using the port
   sudo lsof -i :5221  # or :8080, :80
   
   # Kill the process or change port in docker-compose files
   ```

2. **Permission Denied**
   ```bash
   # Make scripts executable
   chmod +x deploy.sh cleanup.sh
   
   # Fix Docker permissions (Linux)
   sudo usermod -aG docker $USER
   newgrp docker
   ```

3. **Database Issues**
   ```bash
   # Reset database (removes all data)
   ./cleanup.sh volumes
   ./deploy.sh dev  # or prod
   ```

4. **Container Won't Start**
   ```bash
   # Check logs
   docker-compose logs electronics-store
   
   # Check container status
   docker-compose ps
   ```

### Performance Optimization

1. **Increase Memory Limit**
   ```yaml
   # Add to docker-compose.yml
   services:
     electronics-store:
       deploy:
         resources:
           limits:
             memory: 1G
           reservations:
             memory: 512M
   ```

2. **Enable Health Checks**
   ```yaml
   # Add to docker-compose.yml
   services:
     electronics-store:
       healthcheck:
         test: ["CMD", "curl", "-f", "http://localhost:80/health"]
         interval: 30s
         timeout: 10s
         retries: 3
   ```

## Production Considerations

### SSL/HTTPS Setup
1. Obtain SSL certificates
2. Place certificates in `./ssl/` directory
3. Uncomment HTTPS section in `nginx.conf`
4. Update domain name in nginx configuration

### Security
- Change default ports
- Configure firewall rules
- Use environment files for secrets
- Enable rate limiting (already configured)
- Regular security updates

### Monitoring
- Set up log aggregation
- Configure health checks
- Monitor resource usage
- Set up alerts

## Support

For issues and questions:
1. Check the logs first
2. Review this documentation
3. Check Docker and Docker Compose documentation
4. Create an issue in the repository
