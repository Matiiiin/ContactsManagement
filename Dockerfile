FROM mcr.microsoft.com/dotnet/sdk:9.0 as build
RUN apt-get update && dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

RUN mkdir src
WORKDIR /src

COPY ["src/ContactsManagement.Core/ContactsManagement.Core.csproj", "ContactsManagement.Core/"]
COPY ["src/ContactsManagement.Infrastructure/ContactsManagement.Infrastructure.csproj", "ContactsManagement.Infrastructure/"]
COPY ["src/ContactsManagement.UI/ContactsManagement.UI.csproj", "ContactsManagement.UI/"]
#COPY ["src/ContactsManagement.WebApi.UI/ContactsManagement.WebApi.UI.csproj", "ContactsManagement.WebApi.UI/"]
COPY ./src .

RUN dotnet restore ContactsManagement.UI/ContactsManagement.UI.csproj

WORKDIR /src/ContactsManagement.UI
RUN dotnet ef migrations add Initial --project /src/ContactsManagement.Infrastructure --startup-project /src/ContactsManagement.UI
RUN dotnet build ContactsManagement.UI.csproj -c Release -o /app/build


FROM build AS publish
RUN dotnet publish --no-restore -c Release -o /app/publish

FROM publish AS final

WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 8001
CMD ["sh", "-c", "dotnet ef database update --project /src/ContactsManagement.Infrastructure --startup-project /src/ContactsManagement.UI && dotnet ContactsManagement.UI.dll"]