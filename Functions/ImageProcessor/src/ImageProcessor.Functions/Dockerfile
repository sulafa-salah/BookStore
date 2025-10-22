FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated8.0 AS base
WORKDIR /home/site/wwwroot
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# 1) copy sln + the EXACT csproj path (from repo root)
COPY BookStore.sln ./
COPY Functions/ImageProcessor/src/ImageProcessor.Functions/ImageProcessor.Functions.csproj Functions/ImageProcessor/src/ImageProcessor.Functions/

# (add COPY lines for any other referenced .csproj if applicable)

# 2) restore
RUN dotnet restore Functions/ImageProcessor/src/ImageProcessor.Functions/ImageProcessor.Functions.csproj

# 3) copy source
COPY . .

# 4) build & publish
WORKDIR /src/Functions/ImageProcessor/src/ImageProcessor.Functions
RUN dotnet build ImageProcessor.Functions.csproj -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish ImageProcessor.Functions.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated8.0 AS final
WORKDIR /home/site/wwwroot
COPY --from=publish /app/publish .
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true