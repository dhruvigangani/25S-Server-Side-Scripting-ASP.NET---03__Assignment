# Use the official ASP.NET runtime as the base image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy everything
COPY . .

# Restore using the correct path to .csproj
RUN dotnet restore "ShiftSchedularApplication/ShiftSchedularApplication.csproj"

# Build and publish
RUN dotnet publish "ShiftSchedularApplication/ShiftSchedularApplication.csproj" -c Release -o /app/publish

# Final image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ShiftSchedularApplication.dll"]
