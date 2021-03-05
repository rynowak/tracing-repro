#!/bin/sh

dapr run --app-id weather-http \
    --app-port 5002 \
    --app-protocol http \
	--dapr-http-port 3504 \
    --log-level debug \
    dotnet run dotnet -- -p ./WeatherMicroservice/WeatherMicroservice.csproj --urls http://localhost:5002