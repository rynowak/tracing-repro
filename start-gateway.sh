#!/bin/sh

dapr run --app-id apigateway \
    --app-port 5000 \
	--dapr-http-port 3500 \
	--dapr-grpc-port 3501 \
    --app-protocol grpc \
    --log-level debug \
    dotnet run dotnet -- -p ./ApiGateway/ApiGateway.csproj