# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the csproj file and restore any dependencies (via npm)
COPY *.csproj ./
RUN dotnet restore

# Copy the rest of the application code
COPY . ./
RUN dotnet publish -c Release -o out

# Use the official ASP.NET runtime image for the runtime environment
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .
# Install curl for network diagnostics
RUN apt-get update && apt-get install -y curl iputils-ping

# Set the entry point for the application
ENTRYPOINT ["dotnet", "sws.dll"]