# ===== Base runtime =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# ===== Build =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files first (layer caching)
COPY Services/Cart/src/Cart.Domain/Cart.Domain.csproj Services/Cart/src/Cart.Domain/
COPY Services/Cart/src/Cart.Application/Cart.Application.csproj Services/Cart/src/Cart.Application/
COPY Services/Cart/src/Cart.Infrastructure/Cart.Infrastructure.csproj Services/Cart/src/Cart.Infrastructure/
COPY Services/Cart/src/Cart.Contracts/Cart.Contracts.csproj Services/Cart/src/Cart.Contracts/
COPY Services/Cart/src/Cart.Api/Cart.Api.csproj Services/Cart/src/Cart.Api/

# Shared libs
COPY Shared/SharedKernel/SharedKernel.csproj Shared/SharedKernel/
COPY Shared/Auth/Permissions/BookStore.Permissions.csproj Shared/Auth/Permissions/

RUN dotnet restore "Services/Cart/src/Cart.Api/Cart.Api.csproj"

# Copy the rest
COPY . .
WORKDIR /src/Services/Cart/src/Cart.Api
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Cart.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# ===== Final =====
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet","Cart.Api.dll"]