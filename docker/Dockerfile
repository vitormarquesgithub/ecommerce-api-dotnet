# ──────────────── build ────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["ECommerce.sln", "./"]
COPY ["ECommerce.csproj", "./"]

COPY ["src/", "./src/"]
COPY ["tests/", "./tests/"]

RUN dotnet restore "ECommerce.sln"

RUN dotnet publish "ECommerce.sln" \
    -c Release \
    -o /app/publish \
    --no-restore

# ──────────────── runtime ────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# ──────────────── metadata ────────────────
LABEL org.opencontainers.image.title="ECommerce API"
LABEL org.opencontainers.image.description="ECommerce system API built with .NET 8"
LABEL org.opencontainers.image.version="1.0.0"
LABEL maintainer="youremail@example.com"

# ──────────────── arquivos e portas ────────────────
COPY --from=build /app/publish ./
EXPOSE 80
EXPOSE 443

# ──────────────── healthcheck ────────────────
HEALTHCHECK --interval=30s --timeout=5s --start-period=10s \
  CMD curl --silent --fail http://localhost/health || exit 1

# ──────────────── entrypoint ────────────────
ENTRYPOINT ["dotnet", "ECommerce.dll"]
