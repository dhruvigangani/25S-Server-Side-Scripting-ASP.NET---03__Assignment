# Base image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Build image
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy everything
COPY . /src

# SET working directory to project folder
WORKDIR /src/ShiftSchedularApplication

# Restore using csproj
RUN dotnet restore "ShiftSchedularApplication.csproj"

# Build and publish
RUN dotnet publish "ShiftSchedularApplication.csproj" -c Release -o /app/publish

# Final image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ShiftSchedularApplication.dll"]