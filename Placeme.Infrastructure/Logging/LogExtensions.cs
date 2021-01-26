using System;
using System.Diagnostics;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Events;
using Serilog.Formatting.Elasticsearch;

namespace Placeme.Infrastructure.Logging
{
    public static class LogExtensions
    {
        private static LogEventLevel GetLogEventLevel()
        {
            var logLevel = LogEventLevel.Information;
            var desiredLogLevel = Environment.GetEnvironmentVariable("LOG_LEVEL");

            if (!string.IsNullOrEmpty(desiredLogLevel))
            {
                if (!Enum.TryParse(desiredLogLevel, out LogEventLevel parsedLogLevel))
                {
                    Trace.TraceWarning("Error parsing Serilog.LogEventLevel. Defaulting to {0}", logLevel);
                    logLevel = parsedLogLevel;
                }
            }

            return logLevel;
        }

        public static LoggerConfiguration CreateLoggerConfiguration(bool isDevelopment = false)
        {
            Activity.DefaultIdFormat = ActivityIdFormat.W3C;
            var logLevel = GetLogEventLevel();

            var config = new LoggerConfiguration()
                .MinimumLevel.Is(logLevel)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .Enrich.WithSpan()
                .Enrich.FromLogContext();
                
            if (isDevelopment)
            {
                config.WriteTo.Console();
            }
            else
            {
                config.WriteTo.Console(new ElasticsearchJsonFormatter());
            }

            return config;
        }
    }
}