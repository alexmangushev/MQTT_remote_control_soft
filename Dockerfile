#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["mqtt_remote_server.csproj", "."]
RUN dotnet restore "./mqtt_remote_server.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "mqtt_remote_server.csproj" -c Debug -o /app/build

FROM build AS publish
RUN dotnet publish "mqtt_remote_server.csproj" -c Debug -o /app/publish /p:UseAppHost=false

COPY ConnectionMQTT.txt /app/publish
COPY ConnectionDataBase.txt /app/publish
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "mqtt_remote_server.dll"]