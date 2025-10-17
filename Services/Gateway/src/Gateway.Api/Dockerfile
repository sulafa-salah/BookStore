# build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy only the csproj first (better layer caching)
COPY Services/Gateway/src/Gateway.Api/Gateway.Api.csproj Services/Gateway/src/Gateway.Api/
RUN dotnet restore Services/Gateway/src/Gateway.Api/Gateway.Api.csproj

# now copy the rest of the source
COPY Services/Gateway/src/Gateway.Api/ Services/Gateway/src/Gateway.Api/

WORKDIR /src/Services/Gateway/src/Gateway.Api
RUN dotnet publish Gateway.Api.csproj -c Release -o /app /p:UseAppHost=false

# runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "Gateway.Api.dll"]