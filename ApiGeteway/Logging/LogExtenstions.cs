using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Formatting.Elasticsearch;

namespace ApiGeteway.Logging
{
    public static class LogExtenstions
    {
        public static void CreateLoggerConfiguration(this IServiceProvider serviceProvider, bool isRunOnTye = true)
        {
            if (isRunOnTye) return;

            var config = serviceProvider.GetService<IConfiguration>();

            var loggerConfig = new LoggerConfiguration()
                .ReadFrom.Configuration(config, "Logging")
                .Enrich.WithSpan()
                .Enrich.FromLogContext()
                .WriteTo.Console(new ElasticsearchJsonFormatter());

            Log.Logger = loggerConfig.CreateLogger();
        }
    }
}