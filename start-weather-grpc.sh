#!/bin/sh

dapr run --app-id weather-grpc \
    --app-port 5001 \
    --app-protocol grpc \
	--dapr-http-port 3502 \
	--dapr-grpc-port 3503 \
    --log-level debug
