FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/Api/Todo.Api.csproj src/Api/
COPY src/ServiceDefaults/ServiceDefaults.csproj src/ServiceDefaults/
RUN dotnet restore src/Api/Todo.Api.csproj

COPY src/Api/ src/Api/
COPY src/ServiceDefaults/ src/ServiceDefaults/
RUN dotnet publish src/Api/Todo.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 5137
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Todo.Api.dll"]
