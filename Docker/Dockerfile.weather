FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build

WORKDIR /src

COPY WeatherMicroservice.csproj .
RUN dotnet restore

COPY . .

RUN dotnet publish --no-restore -c Release -o /published WeatherMicroservice.csproj

FROM mcr.microsoft.com/dotnet/aspnet:5.0 as runtime
WORKDIR /app
#ENV ASPNETCORE_URLS=http://+:5001;http://+:5002
#ENV ASPNETCORE_ENVIRONMENT=”production”
EXPOSE 5001
EXPOSE 5002
COPY --from=build /published .
ENTRYPOINT [ "dotnet", "WeatherMicroservice.dll" ]