# Base image with .NET 8.0 runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# SDK image to build the app using .NET 8.0
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the entire project
COPY . .

# Restore dependencies
RUN dotnet restore "ShiftSchedularApplication.csproj"

# Build and publish the app
RUN dotnet publish "ShiftSchedularApplication.csproj" -c Release -o /app/publish

# Final stage - runtime image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

# Entry point
ENTRYPOINT ["dotnet", "ShiftSchedularApplication.dll"]