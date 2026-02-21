# Estágio 1: Build da Aplicação
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source

# 1. Copia PRIMEIRO o projeto Shared para otimização de cache
COPY ["agro-shared/AgroSolutions.Shared/AgroSolutions.Shared.csproj", "agro-shared/AgroSolutions.Shared/"]

# 2. Copia os arquivos de projeto do Identity
COPY ["agro-identity/AgroSolutions.Identity.API/AgroSolutions.Identity.API.csproj", "agro-identity/AgroSolutions.Identity.API/"]
COPY ["agro-identity/AgroSolutions.Identity.Application/AgroSolutions.Identity.Application.csproj", "agro-identity/AgroSolutions.Identity.Application/"]
COPY ["agro-identity/AgroSolutions.Identity.Domain/AgroSolutions.Identity.Domain.csproj", "agro-identity/AgroSolutions.Identity.Domain/"]
COPY ["agro-identity/AgroSolutions.Identity.Infrastructure/AgroSolutions.Identity.Infrastructure.csproj", "agro-identity/AgroSolutions.Identity.Infrastructure/"]

# 3. Restaura as dependências a partir do projeto principal (API)
RUN dotnet restore "agro-identity/AgroSolutions.Identity.API/AgroSolutions.Identity.API.csproj"

# 4. Copia o restante do código-fonte
COPY agro-shared/ agro-shared/
COPY agro-identity/ agro-identity/

# 5. Publica a aplicação
WORKDIR /source/agro-identity/AgroSolutions.Identity.API
RUN dotnet publish "AgroSolutions.Identity.API.csproj" -c Release -o /app/publish --no-restore

# Estágio 2: Criação da Imagem Final (Otimizada com Alpine)
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine
WORKDIR /app

# Instala dependências de globalização
RUN apk add --no-cache icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Copia os artefatos do estágio de build
COPY --from=build /app/publish .

# Dá permissão ao usuário 'app' para que o SQLite consiga criar o arquivo .db
RUN chown -R app:app /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

USER app
ENTRYPOINT ["dotnet", "AgroSolutions.Identity.API.dll"]