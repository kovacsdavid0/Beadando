FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY API/API.csproj API/
COPY EatAndRate.Shared/EatAndRate.Shared.csproj EatAndRate.Shared/
RUN dotnet restore API/API.csproj

COPY API/ API/
COPY EatAndRate.Shared/ EatAndRate.Shared/
RUN dotnet publish API/API.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet", "API.dll"]