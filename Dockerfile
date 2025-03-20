FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["WebAppWithHighSecurity/WebAppWithHighSecurity.csproj", "WebAppWithHighSecurity/"]
RUN dotnet restore "WebAppWithHighSecurity/WebAppWithHighSecurity.csproj"
COPY . .
WORKDIR "/src/WebAppWithHighSecurity"
RUN dotnet build "WebAppWithHighSecurity.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "WebAppWithHighSecurity.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebAppWithHighSecurity.dll"]
