FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /working
COPY ["LoggingTrial/LoggingTrial.csproj", "LoggingTrial/"]
RUN dotnet restore "./LoggingTrial/LoggingTrial.csproj"
COPY . .
WORKDIR "/working/LoggingTrial/"
RUN dotnet build "./LoggingTrial.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./LoggingTrial.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LoggingTrial.dll"]
