# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src

# copy everything and restore as distinct layers
COPY . .
WORKDIR "/src/CinemaAPI"
RUN dotnet build "CinemaAPI.csproj" -c Release -o /app/build

# publish app
FROM build AS publish
RUN dotnet publish "CinemaAPI.csproj" -c Release -o /app/publish

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CinemaAPI.dll"]
