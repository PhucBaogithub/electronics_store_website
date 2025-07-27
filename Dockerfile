# Use the official .NET 9.0 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy csproj and restore dependencies
COPY ElectronicsStore.csproj ./
RUN dotnet restore ElectronicsStore.csproj

# Copy everything else and build
COPY . ./
RUN dotnet publish ElectronicsStore.csproj -c Release -o out

# Use the official .NET 9.0 runtime image for running
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Install SQLite (if needed for runtime)
RUN apt-get update && apt-get install -y sqlite3 && rm -rf /var/lib/apt/lists/*

# Copy the published app
COPY --from=build /app/out .

# Create directory for SQLite database
RUN mkdir -p /app/Data

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80

# Expose port
EXPOSE 80

# Set the entry point
ENTRYPOINT ["dotnet", "ElectronicsStore.dll"]
