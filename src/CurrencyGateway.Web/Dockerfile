FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/CurrencyGateway.Web/CurrencyGateway.Web.csproj", "src/CurrencyGateway.Web/"]
COPY ["src/CurrencyGateway.Domain/CurrencyGateway.Domain.csproj", "src/CurrencyGateway.Domain/"]
COPY ["src/CurrencyGateway.Application/CurrencyGateway.Application.csproj", "src/CurrencyGateway.Application/"]
COPY ["src/CurrencyGateway.Presentation/CurrencyGateway.Presentation.csproj", "src/CurrencyGateway.Presentation/"]
COPY ["src/CurrencyGateway.Infrastructure/CurrencyGateway.Infrastructure.csproj", "src/CurrencyGateway.Infrastructure/"]
RUN dotnet restore "src/CurrencyGateway.Web/CurrencyGateway.Web.csproj"
COPY . .
WORKDIR "/src/src/CurrencyGateway.Web"
RUN dotnet build "CurrencyGateway.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "CurrencyGateway.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CurrencyGateway.Web.dll"]

