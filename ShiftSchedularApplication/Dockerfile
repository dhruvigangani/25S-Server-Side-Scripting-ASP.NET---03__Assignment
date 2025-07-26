# Use the official ASP.NET runtime as base
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy project and restore
COPY . .
RUN dotnet restore "ShiftSchedularApplication.csproj"
RUN dotnet publish "ShiftSchedularApplication.csproj" -c Release -o /app/publish

# Build final image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ShiftSchedularApplication.dll"]
