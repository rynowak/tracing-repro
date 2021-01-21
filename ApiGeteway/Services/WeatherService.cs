﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ApiGeteway.Models;
using Dapr.Client;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using WeatherMicroservice.Services;

namespace ApiGeteway.Services
{
    public class WeatherService : IWeatherService
    {
        private const string WeatherHttp = "weather-http";
        private const string WeatherGrpc = "weather-grpc";
        private const int DaprHttpPort = 3500;
        private readonly DaprClient _dapr;
        private readonly string _daprUrl = $"http://localhost:{DaprHttpPort}/v1.0/invoke/{WeatherHttp}";
        private ILogger<WeatherService> _logger;

        public WeatherService(ILoggerFactory loggerFactory, DaprClient dapr)
        {
            _logger = loggerFactory.CreateLogger<WeatherService>();
            _dapr = dapr;
        }

        public async Task<WeatherReply> GetForecastsByGrpc()
        {
            var channel = GrpcChannel.ForAddress("http://localhost:5001");
            var client = new Weather.WeatherClient(channel);
            var response = await client.GetForecastAsync(new Empty());

            return response;
        }

        public async Task<IEnumerable<WeatherForecastDto>> GetForecastsByRest()
        {
            var httpClient = new HttpClient();
            var streamAsync = httpClient.GetStreamAsync($"{_daprUrl}/method/Weather");
            var result = await JsonSerializer.DeserializeAsync<IEnumerable<WeatherForecastDto>>(await streamAsync);

            return result;
        }

        public async Task<IEnumerable<WeatherForecastDto>> GetForecastsByDaprGrpc()
        {
            Console.WriteLine("Invoking grpc weather forecasts");

            try
            {
                var res = await _dapr.InvokeMethodAsync<IEnumerable<WeatherForecastDto>>(WeatherGrpc, "GetForecast");
                return res;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return null;
        }
    }
}