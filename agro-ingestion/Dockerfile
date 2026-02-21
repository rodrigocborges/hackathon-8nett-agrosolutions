FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source

# 1. Copia projeto Shared
COPY ["agro-shared/AgroSolutions.Shared/AgroSolutions.Shared.csproj", "agro-shared/AgroSolutions.Shared/"]

# 2. Copia projetos do Ingestion
COPY ["agro-ingestion/AgroSolutions.Ingestion.API/AgroSolutions.Ingestion.API.csproj", "agro-ingestion/AgroSolutions.Ingestion.API/"]
COPY ["agro-ingestion/AgroSolutions.Ingestion.Application/AgroSolutions.Ingestion.Application.csproj", "agro-ingestion/AgroSolutions.Ingestion.Application/"]
COPY ["agro-ingestion/AgroSolutions.Ingestion.Domain/AgroSolutions.Ingestion.Domain.csproj", "agro-ingestion/AgroSolutions.Ingestion.Domain/"]
COPY ["agro-ingestion/AgroSolutions.Ingestion.Infrastructure/AgroSolutions.Ingestion.Infrastructure.csproj", "agro-ingestion/AgroSolutions.Ingestion.Infrastructure/"]

# 3. Restaura dependências
RUN dotnet restore "agro-ingestion/AgroSolutions.Ingestion.API/AgroSolutions.Ingestion.API.csproj"

# 4. Copia código-fonte
COPY agro-shared/ agro-shared/
COPY agro-ingestion/ agro-ingestion/

# 5. Publica
WORKDIR /source/agro-ingestion/AgroSolutions.Ingestion.API
RUN dotnet publish "AgroSolutions.Ingestion.API.csproj" -c Release -o /app/publish --no-restore

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
ENTRYPOINT ["dotnet", "AgroSolutions.Ingestion.API.dll"]