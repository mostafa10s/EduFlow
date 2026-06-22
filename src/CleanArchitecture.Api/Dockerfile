# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files first for layer caching
COPY src/CleanArchitecture.Domain/*.csproj                 src/CleanArchitecture.Domain/
COPY src/CleanArchitecture.Application/*.csproj           src/CleanArchitecture.Application/
COPY src/CleanArchitecture.Infrastructure/*.csproj        src/CleanArchitecture.Infrastructure/
COPY src/CleanArchitecture.Api/*.csproj                   src/CleanArchitecture.Api/
COPY tests/CleanArchitecture.Tests/*.csproj               tests/CleanArchitecture.Tests/
COPY CleanArchitecture.slnx ./

RUN dotnet restore src/CleanArchitecture.Api/CleanArchitecture.Api.csproj

# Copy everything else and publish
COPY . .
RUN dotnet publish src/CleanArchitecture.Api/CleanArchitecture.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "CleanArchitecture.Api.dll"]
