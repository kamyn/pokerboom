#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["PokerBoom/Server/PokerBoom.Server.csproj", "PokerBoom/Server/"]
COPY ["PokerBoom/Client/PokerBoom.Client.csproj", "PokerBoom/Client/"]
COPY ["PokerBoom/Shared/PokerBoom.Shared.csproj", "PokerBoom/Shared/"]
RUN dotnet restore "PokerBoom/Server/PokerBoom.Server.csproj"
COPY . .
WORKDIR "/src/PokerBoom/Server"
RUN dotnet build "PokerBoom.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PokerBoom.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PokerBoom.Server.dll"]