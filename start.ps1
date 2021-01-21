Start-Process pwsh.exe -argument '-nologo -noprofile -executionpolicy bypass -command `
    dapr run --app-id apigateway `
    --app-port 5000 `
	--dapr-http-port 3500 `
	--dapr-grpc-port 3501 `
    --app-protocol grpc `
    --log-level debug `
    dotnet run dotnet -- -p .\ApiGeteway\ApiGeteway.csproj'
Start-Sleep -Seconds 1

Start-Process pwsh.exe -argument '-nologo -noprofile -executionpolicy bypass -command `
    dapr run --app-id weather-grpc `
    --app-port 5001 `
    --app-protocol grpc `
	--dapr-http-port 3502 `
	--dapr-grpc-port 3503 `
    --log-level debug `
    dotnet run dotnet -- -p .\WeatherMicroservice\WeatherMicroservice.csproj'

Start-Process pwsh.exe -argument '-nologo -noprofile -executionpolicy bypass -command `
    dapr run --app-id weather-http `
    --app-port 5002 `
    --app-protocol http `
	--dapr-http-port 3504 `
    --log-level debug `
    dotnet run dotnet -- -p .\WeatherMicroservice\WeatherMicroservice.csproj'
Start-Sleep -Seconds 1
#Start-Process -NoNewWindow dapr -argument 'run --app-id weather --app-port 5001 dotnet run dotnet -- -p .\WeatherMicroservice\WeatherMicroservice.csproj'
