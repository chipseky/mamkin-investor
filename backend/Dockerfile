FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /mamkin-investor

COPY Mamkin.In.Infrastructure/Mamkin.In.Infrastructure.csproj Mamkin.In.Infrastructure/
COPY Mamkin.In.WebApi/Mamkin.In.WebApi.csproj Mamkin.In.WebApi/
COPY Mamkin.In.Domain/Mamkin.In.Domain.csproj Mamkin.In.Domain/

RUN dotnet restore Mamkin.In.WebApi/Mamkin.In.WebApi.csproj

COPY ./Mamkin.In.Infrastructure ./Mamkin.In.Infrastructure
COPY ./Mamkin.In.WebApi ./Mamkin.In.WebApi
COPY ./Mamkin.In.Domain ./Mamkin.In.Domain

RUN dotnet publish Mamkin.In.WebApi/Mamkin.In.WebApi.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app .
ENTRYPOINT ["dotnet", "Mamkin.In.WebApi.dll"]