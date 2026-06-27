# =========================
# BUILD STAGE
# =========================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Copiar solution e projetos
COPY . .

# Restore
RUN dotnet restore

# Build + Publish
RUN dotnet publish DMS.Api/DMS.Api.csproj -c Release -o /app/publish


# =========================
# RUNTIME STAGE
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

# Copiar publish
COPY --from=build /app/publish .

# Expor porta da API
EXPOSE 8080

# Start da aplicação
ENTRYPOINT ["dotnet", "DMS.Api.dll"]