FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /mamkin-investor

COPY Chipseky.MamkinInvestor.WebApi/Chipseky.MamkinInvestor.WebApi.csproj Chipseky.MamkinInvestor.WebApi/
COPY Chipseky.MamkinInvestor.Domain/Chipseky.MamkinInvestor.Domain.csproj Chipseky.MamkinInvestor.Domain/

RUN dotnet restore Chipseky.MamkinInvestor.WebApi/Chipseky.MamkinInvestor.WebApi.csproj

COPY ./Chipseky.MamkinInvestor.WebApi ./Chipseky.MamkinInvestor.WebApi
COPY ./Chipseky.MamkinInvestor.Domain ./Chipseky.MamkinInvestor.Domain

RUN dotnet publish Chipseky.MamkinInvestor.WebApi/Chipseky.MamkinInvestor.WebApi.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app .
ENTRYPOINT ["dotnet", "Chipseky.MamkinInvestor.WebApi.dll"]