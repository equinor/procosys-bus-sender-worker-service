#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Equinor.ProCoSys.BusSender.Worker/Equinor.ProCoSys.BusSender.Worker.csproj", "src/Equinor.ProCoSys.BusSender.Worker/"]
COPY ["src/Equinor.ProCoSys.BusSender.Infrastructure/Equinor.ProCoSys.BusSenderWorker.Infrastructure.csproj", "src/Equinor.ProCoSys.BusSender.Infrastructure/"]
COPY ["src/Equinor.ProCoSys.PcsBus/Equinor.ProCoSys.PcsServiceBus.csproj", "src/Equinor.ProCoSys.PcsBus/"]
COPY ["src/Equinor.ProCoSys.BusSender.Core/Equinor.ProCoSys.BusSenderWorker.Core.csproj", "src/Equinor.ProCoSys.BusSender.Core/"]
RUN dotnet restore "src/Equinor.ProCoSys.BusSender.Worker/Equinor.ProCoSys.BusSender.Worker.csproj"
COPY . .
WORKDIR "/src/src/Equinor.ProCoSys.BusSender.Worker"
RUN dotnet build "Equinor.ProCoSys.BusSender.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Equinor.ProCoSys.BusSender.Worker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Equinor.ProCoSys.BusSender.Worker.dll"]