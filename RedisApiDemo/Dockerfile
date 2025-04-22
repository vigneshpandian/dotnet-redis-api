# ---------- Base Image for Runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# ---------- Build Stage ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project and restore dependencies
COPY ["RedisApiDemo/RedisApiDemo.csproj", "RedisApiDemo/"]
RUN dotnet restore "./RedisApiDemo/RedisApiDemo.csproj"

# Copy the rest of the code
COPY . .
WORKDIR "/src/RedisApiDemo"
RUN dotnet build "./RedisApiDemo.csproj" -c $BUILD_CONFIGURATION -o /app/build

# ---------- Publish Stage ----------
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./RedisApiDemo.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# ---------- Final Runtime Image ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

# Install curl (requires root temporarily)
USER root
RUN apt-get update && apt-get install -y curl && apt-get clean

# Switch back to non-root user after installing curl
USER app

WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "RedisApiDemo.dll"]
