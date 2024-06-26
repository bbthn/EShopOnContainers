#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 5003

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/Services/BasketService/BasketService.Api/BasketService.Api.csproj", "src/Services/BasketService/BasketService.Api/"]
COPY ["src/BuildingBlocks/EventBus/EventBus.Base/EventBus.Base.csproj", "src/BuildingBlocks/EventBus/EventBus.Base/"]
COPY ["src/BuildingBlocks/EventBus/EventBus.Factory/EventBus.Factory.csproj", "src/BuildingBlocks/EventBus/EventBus.Factory/"]
COPY ["src/BuildingBlocks/EventBus/EventBus.AzureServiceBus/EventBus.AzureServiceBus.csproj", "src/BuildingBlocks/EventBus/EventBus.AzureServiceBus/"]
COPY ["src/BuildingBlocks/EventBus/EventBus.RabbitMQ/EventBus.RabbitMQ.csproj", "src/BuildingBlocks/EventBus/EventBus.RabbitMQ/"]
RUN dotnet restore "src/Services/BasketService/BasketService.Api/BasketService.Api.csproj"
COPY . .
WORKDIR "/src/src/Services/BasketService/BasketService.Api"
RUN dotnet build "BasketService.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BasketService.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BasketService.Api.dll"]