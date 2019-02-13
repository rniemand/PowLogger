#FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
FROM microsoft/dotnet:2.2.2-runtime-alpine AS base

WORKDIR /app

#FROM microsoft/dotnet:2.2-sdk AS build
FROM microsoft/dotnet:2.2-sdk-alpine AS build

WORKDIR /src
COPY ["src/PowLogger/PowLogger.csproj", "PowLogger/"]
RUN dotnet restore "PowLogger/PowLogger.csproj"

WORKDIR "/src/PowLogger"
COPY src/PowLogger/ .
RUN dotnet build "PowLogger.csproj" -c Release -o /app

VOLUME /app/config

WORKDIR /app
ENTRYPOINT ["dotnet", "PowLogger.dll"]