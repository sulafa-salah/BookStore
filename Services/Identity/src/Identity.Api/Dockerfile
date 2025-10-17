# ===== Base runtime =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# ===== Build =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files first (layer caching)
COPY Services/Identity/src/Identity.Domain/Identity.Domain.csproj Services/Identity/src/Identity.Domain/
COPY Services/Identity/src/Identity.Application/Identity.Application.csproj Services/Identity/src/Identity.Application/
COPY Services/Identity/src/Identity.Infrastructure/Identity.Infrastructure.csproj Services/Identity/src/Identity.Infrastructure/
COPY Services/Identity/src/Identity.Contracts/Identity.Contracts.csproj Services/Identity/src/Identity.Contracts/
COPY Services/Identity/src/Identity.Api/Identity.Api.csproj Services/Identity/src/Identity.Api/

# Shared libs used across services
COPY Shared/SharedKernel/SharedKernel.csproj Shared/SharedKernel/
COPY Shared/Auth/Permissions/BookStore.Permissions.csproj Shared/Auth/Permissions/

RUN dotnet restore "Services/Identity/src/Identity.Api/Identity.Api.csproj"

# Copy the rest of the source
COPY . .
WORKDIR /src/Services/Identity/src/Identity.Api
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Identity.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# ===== Final =====
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet","Identity.Api.dll"]