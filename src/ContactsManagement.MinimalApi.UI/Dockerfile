FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ContactsManagement.MinimalApi.UI/ContactsManagement.MinimalApi.UI.csproj", "ContactsManagement.MinimalApi.UI/"]
RUN dotnet restore "ContactsManagement.MinimalApi.UI/ContactsManagement.MinimalApi.UI.csproj"
COPY . .
WORKDIR "/src/ContactsManagement.MinimalApi.UI"
RUN dotnet build "ContactsManagement.MinimalApi.UI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ContactsManagement.MinimalApi.UI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ContactsManagement.MinimalApi.UI.dll"]
