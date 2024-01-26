#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SmaugX.Host/SmaugX.Host.csproj", "SmaugX.Host/"]
COPY ["SmaugX.Core/SmaugX.Core.csproj", "SmaugX.Core/"]

RUN dotnet restore "./SmaugX.Host/./SmaugX.Host.csproj"
COPY . .
WORKDIR "/src/SmaugX.Host"
RUN dotnet build "./SmaugX.Host.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SmaugX.Host.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SmaugX.Host.dll"]