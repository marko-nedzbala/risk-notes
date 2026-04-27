FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

COPY *.csproj .

RUN dotnet restore

COPY . .

RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/donet/aspnet:9.0 as runtime

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "RiskNotes.dll"]



