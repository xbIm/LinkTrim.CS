FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app

ENV ASPNETCORE_HTTP_PORTS=5000
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["LinkTrim.Web/LinkTrim.Web.csproj", "LinkTrim.Web/"]
RUN dotnet restore "LinkTrim.Web/LinkTrim.Web.csproj"
COPY . .
WORKDIR "/src/LinkTrim.Web"
RUN dotnet build "LinkTrim.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "LinkTrim.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LinkTrim.Web.dll"]
