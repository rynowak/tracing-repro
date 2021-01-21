FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
#COPY ["placeme-dapr-example.sln", "./"]
#COPY ["ApiGeteway/ApiGeteway.csproj", "./ApiGeteway/"]
COPY ["WeatherMicroservice/WeatherMicroservice.csproj", "./WeatherMicroservice/"]
#COPY . .
#RUN dotnet restore "placeme-dapr-example.sln"
RUN dotnet restore "WeatherMicroservice/WeatherMicroservice.csproj"
RUN mkdir /dist
RUN dotnet build "WeatherMicroservice/WeatherMicroservice.csproj" -c Release -o /dist
#RUN dotnet build "placeme-dapr-example.sln" -c Release -o /app/build
#
#FROM build AS publish
#RUN dotnet publish "IdentityServer.csproj" -c Release -o /app/publish
#
#FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine AS base
#RUN apk add icu-libs
#ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
#WORKDIR /app
#EXPOSE 80
#
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#VOLUME [ "/app/config" ]
#ENTRYPOINT ["dotnet", "IdentityServer.dll"]