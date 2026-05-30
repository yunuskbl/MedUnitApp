FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files first — cached unless .csproj files change
COPY ["RandevuAPI/RandevuAPI.csproj", "RandevuAPI/"]
COPY ["MedUnit.Application/MedUnit.Application.csproj", "MedUnit.Application/"]
COPY ["MedUnit.Domain/MedUnit.Domain.csproj", "MedUnit.Domain/"]
COPY ["MedUnit.Infrastructure/MedUnit.Infrastructure.csproj", "MedUnit.Infrastructure/"]
RUN dotnet restore "RandevuAPI/RandevuAPI.csproj"

# Copy full source — only invalidates the publish layer
COPY . .
RUN dotnet publish "RandevuAPI/RandevuAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "RandevuAPI.dll"]