#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["cleanshop1/cleanshop1.csproj", "cleanshop1/"]
RUN dotnet restore "cleanshop1/cleanshop1.csproj"
COPY . .
WORKDIR "/src/cleanshop1"
RUN dotnet build "cleanshop1.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "cleanshop1.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "cleanshop1.dll"]