# ===== Base runtime =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# ===== Build =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files first (layer caching)
COPY Services/Catalog/src/Catalog.Domain/Catalog.Domain.csproj Services/Catalog/src/Catalog.Domain/
COPY Services/Catalog/src/Catalog.Application/Catalog.Application.csproj Services/Catalog/src/Catalog.Application/
COPY Services/Catalog/src/Catalog.Infrastructure/Catalog.Infrastructure.csproj Services/Catalog/src/Catalog.Infrastructure/
COPY Services/Catalog/src/Catalog.Contracts/Catalog.Contracts.csproj Services/Catalog/src/Catalog.Contracts/
COPY Services/Catalog/src/Catalog.Api/Catalog.Api.csproj Services/Catalog/src/Catalog.Api/

# Shared libs
COPY Shared/SharedKernel/SharedKernel.csproj Shared/SharedKernel/
COPY Shared/Auth/Permissions/BookStore.Permissions.csproj Shared/Auth/Permissions/


RUN dotnet restore "Services/Catalog/src/Catalog.Api/Catalog.Api.csproj"

# Copy the rest
COPY . .
WORKDIR /src/Services/Catalog/src/Catalog.Api
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Catalog.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# ===== Final =====
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet","Catalog.Api.dll"]