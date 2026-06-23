# syntax=docker/dockerfile:1.7

# ---------- Stage 1: build ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj files first for layer caching
COPY src/BibleStudy.Domain/*.csproj src/BibleStudy.Domain/
COPY src/BibleStudy.Application/*.csproj src/BibleStudy.Application/
COPY src/BibleStudy.Infrastructure/*.csproj src/BibleStudy.Infrastructure/
COPY src/BibleStudy.Api/*.csproj src/BibleStudy.Api/
RUN dotnet restore src/BibleStudy.Api/BibleStudy.Api.csproj

# Copy the rest and publish
COPY . .
RUN dotnet publish src/BibleStudy.Api/BibleStudy.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ---------- Stage 2: runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish ./
COPY docker-entrypoint.sh /usr/local/bin/docker-entrypoint.sh
RUN chmod +x /usr/local/bin/docker-entrypoint.sh

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["docker-entrypoint.sh"]
CMD ["dotnet", "BibleStudy.Api.dll"]