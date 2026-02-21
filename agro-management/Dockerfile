FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source

# 1. Copia projeto Shared
COPY ["agro-shared/AgroSolutions.Shared/AgroSolutions.Shared.csproj", "agro-shared/AgroSolutions.Shared/"]

# 2. Copia projetos do Management
COPY ["agro-management/AgroSolutions.Management.API/AgroSolutions.Management.API.csproj", "agro-management/AgroSolutions.Management.API/"]
COPY ["agro-management/AgroSolutions.Management.Application/AgroSolutions.Management.Application.csproj", "agro-management/AgroSolutions.Management.Application/"]
COPY ["agro-management/AgroSolutions.Management.Domain/AgroSolutions.Management.Domain.csproj", "agro-management/AgroSolutions.Management.Domain/"]
COPY ["agro-management/AgroSolutions.Management.Infrastructure/AgroSolutions.Management.Infrastructure.csproj", "agro-management/AgroSolutions.Management.Infrastructure/"]

# 3. Restaura dependências
RUN dotnet restore "agro-management/AgroSolutions.Management.API/AgroSolutions.Management.API.csproj"

# 4. Copia código-fonte
COPY agro-shared/ agro-shared/
COPY agro-management/ agro-management/

# 5. Publica
WORKDIR /source/agro-management/AgroSolutions.Management.API
RUN dotnet publish "AgroSolutions.Management.API.csproj" -c Release -o /app/publish --no-restore

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
ENTRYPOINT ["dotnet", "AgroSolutions.Management.API.dll"]