# Use the official .NET runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the .NET SDK to build the application
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "ShiftSchedularApplication.csproj"
RUN dotnet publish "ShiftSchedularApplication.csproj" -c Release -o /app/publish

# Final stage: copy build output
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ShiftSchedularApplication.dll"]