FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/Api/InsurancePolicyManager.Api.csproj", "src/Api/"]
COPY ["src/Application/InsurancePolicyManager.Application.csproj", "src/Application/"]
COPY ["src/Domain/InsurancePolicyManager.Domain.csproj", "src/Domain/"]
COPY ["src/Infrastructure/InsurancePolicyManager.Infrastructure.csproj", "src/Infrastructure/"]
RUN dotnet restore "src/Api/InsurancePolicyManager.Api.csproj"

COPY . .
WORKDIR /src/src/Api
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "InsurancePolicyManager.Api.dll"]
