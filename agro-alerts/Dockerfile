FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source

# 1. Copia projeto Shared
COPY ["agro-shared/AgroSolutions.Shared/AgroSolutions.Shared.csproj", "agro-shared/AgroSolutions.Shared/"]

# 2. Copia projetos do Alerts
COPY ["agro-alerts/AgroSolutions.Alerts.API/AgroSolutions.Alerts.API.csproj", "agro-alerts/AgroSolutions.Alerts.API/"]
COPY ["agro-alerts/AgroSolutions.Alerts.Application/AgroSolutions.Alerts.Application.csproj", "agro-alerts/AgroSolutions.Alerts.Application/"]
COPY ["agro-alerts/AgroSolutions.Alerts.Domain/AgroSolutions.Alerts.Domain.csproj", "agro-alerts/AgroSolutions.Alerts.Domain/"]
COPY ["agro-alerts/AgroSolutions.Alerts.Infrastructure/AgroSolutions.Alerts.Infrastructure.csproj", "agro-alerts/AgroSolutions.Alerts.Infrastructure/"]

# 3. Restaura dependências
RUN dotnet restore "agro-alerts/AgroSolutions.Alerts.API/AgroSolutions.Alerts.API.csproj"

# 4. Copia código-fonte
COPY agro-shared/ agro-shared/
COPY agro-alerts/ agro-alerts/

# 5. Publica
WORKDIR /source/agro-alerts/AgroSolutions.Alerts.API
RUN dotnet publish "AgroSolutions.Alerts.API.csproj" -c Release -o /app/publish --no-restore

# Estágio Final
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine
WORKDIR /app

RUN apk add --no-cache icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

COPY --from=build /app/publish .

RUN chown -R app:app /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

USER app
ENTRYPOINT ["dotnet", "AgroSolutions.Alerts.API.dll"]