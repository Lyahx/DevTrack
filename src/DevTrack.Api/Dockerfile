FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY DevTrack.sln ./
COPY src/DevTrack.Api/DevTrack.Api.csproj src/DevTrack.Api/
COPY src/DevTrack.Domain/DevTrack.Domain.csproj src/DevTrack.Domain/
COPY src/DevTrack.Infrastructure/DevTrack.Infrastructure.csproj src/DevTrack.Infrastructure/
COPY src/DevTrack.Repository/DevTrack.Repository.csproj src/DevTrack.Repository/
COPY src/DevTrack.Service/DevTrack.Service.csproj src/DevTrack.Service/
RUN dotnet restore DevTrack.sln

COPY src/ src/
RUN dotnet publish src/DevTrack.Api/DevTrack.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

ENTRYPOINT ["dotnet", "DevTrack.Api.dll"]
