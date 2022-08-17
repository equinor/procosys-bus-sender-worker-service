#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Equinor.ProCoSys.BusReceiver/Equinor.ProCoSys.BusReceiver.csproj", "src/Equinor.ProCoSys.BusReceiver/"]
RUN dotnet restore "src/Equinor.ProCoSys.BusReceiver/Equinor.ProCoSys.BusReceiver.csproj"
COPY . .
WORKDIR "/src/src/Equinor.ProCoSys.BusReceiver"
RUN dotnet build "Equinor.ProCoSys.BusReceiver.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Equinor.ProCoSys.BusReceiver.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Equinor.ProCoSys.BusReceiver.dll"]