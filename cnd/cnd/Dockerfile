# Base image voor runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Kopieer alleen de projectfile en restore dependencies
COPY ["cnd.csproj", "./"]
RUN dotnet restore "cnd.csproj"

# Kopieer de rest van de source code en build
COPY . .
RUN dotnet build "cnd.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publiceer de app in Release modus
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "cnd.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "cnd.dll"]
