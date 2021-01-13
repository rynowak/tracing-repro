Start-Process pwsh.exe -argument '-nologo -noprofile -executionpolicy bypass -command `
    dapr run --app-id apigateway `
    --app-port 5000 `
    --app-protocol grpc `
    --dapr-grpc-port 3666 `
    --log-level debug `
    dotnet run dotnet -- -p .\ApiGeteway\ApiGeteway.csproj'
Start-Sleep -Seconds 2

Start-Process pwsh.exe -argument '-nologo -noprofile -executionpolicy bypass -command `
    dapr run --app-id weather `
    --app-port 5001 `
    --app-protocol grpc `
    --log-level debug `
    dotnet run dotnet -- -p .\WeatherMicroservice\WeatherMicroservice.csproj'
Start-Sleep -Seconds 2
#Start-Process -NoNewWindow dapr -argument 'run --app-id weather --app-port 5001 dotnet run dotnet -- -p .\WeatherMicroservice\WeatherMicroservice.csproj'
