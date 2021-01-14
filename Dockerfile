FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /app

COPY *.sln ./
COPY Location.IpGeolocation/*.csproj ./Location.IpGeolocation/
COPY Weather/*.csproj ./Weather/
COPY Weather.Console/*.csproj ./Weather.Console/
COPY Weather.Weatherstack/*.csproj ./Weather.Weatherstack/

RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/runtime:3.1
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "Weather.Console.dll"]