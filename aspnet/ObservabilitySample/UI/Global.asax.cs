using ObservabilitySample.Domain.Service;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using System;
using System.Configuration;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;


namespace ObservabilitySample.WebApp
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private TracerProvider tracerProvider;
        private MeterProvider meterProvider;

        protected void Application_Start()
        {
            this.InitializeOpenTelemetryConfigurations();

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_End()
        {
            this.DisposeOpenTelemetry();
        }

        private void InitializeOpenTelemetryConfigurations()
        {
            this.InitializeOpenTelemetryTracer();
            this.InitializeOpenTelemetryMetrics();
        }

        #region Tracer

        private void InitializeOpenTelemetryTracer()
        {
            var builder = Sdk.CreateTracerProviderBuilder()
                                 .AddAspNetInstrumentation()
                                 .AddSource(CustomMetrics.greeterActivitySource.Name);

            switch (GetTracerExporterType())
            {
                //case "ZIPKIN":
                //    builder.AddZipkinExporter(zipkinOptions =>
                //    {
                //        zipkinOptions.Endpoint = new Uri(ConfigurationManager.AppSettings["ZipkinEndpoint"]);
                //    });
                //    break;
                case "OTLP":
                    builder.AddOtlpExporter(otlpOptions =>
                    {
                        otlpOptions.Endpoint = new Uri(GetTracerUrlForOltpExporter());
                    });
                    break;
                default:
                    builder.AddConsoleExporter(options => options.Targets = ConsoleExporterOutputTargets.Debug);
                    break;
            }

            this.tracerProvider = builder.Build();
        }

        private static string GetTracerUrlForOltpExporter()
        {
            return ConfigurationManager.AppSettings["Config.Observability.OpenTelemetry.TracerOtlpEndpoint"];
        }

        private static string GetTracerExporterType()
        {
            return ConfigurationManager.AppSettings["Config.Observability.OpenTelemetry.UseTracerExporter"].ToUpperInvariant();
        }

        #endregion

        #region Metrics

        private void InitializeOpenTelemetryMetrics()
        {
            // Metrics
            // Note: Tracerprovider is needed for metrics to work
            // https://github.com/open-telemetry/opentelemetry-dotnet/issues/2994

            var meterBuilder = Sdk.CreateMeterProviderBuilder()
                 .AddAspNetInstrumentation()
                 .AddMeter(CustomMetrics.greeterMeter.Name);

            switch (GetMetricExporterType())
            {
                case "OTLP":
                    meterBuilder.AddOtlpExporter(otlpOptions =>
                    {
                        otlpOptions.Endpoint = new Uri(GetMetricsUrlForOltpExporter());
                    });
                    break;
                case "PROMETHEUS":
                    meterBuilder.AddPrometheusHttpListener( ); // Default: http://localhost:9464/metrics

                    // Install Prometheus (default port is 9090) and add http://localhost:9464/metrics in "targets list"
                    // Install Graphana from Docker and add Prometheus: http://host.docker.internal:9090
                    break;
                default:
                    meterBuilder.AddConsoleExporter((exporterOptions, metricReaderOptions) =>
                    {
                        exporterOptions.Targets = ConsoleExporterOutputTargets.Debug;
                    });
                    break;
            }

            this.meterProvider = meterBuilder.Build();
        }

        private static string GetMetricsUrlForOltpExporter()
        {
            return ConfigurationManager.AppSettings["Config.Observability.OpenTelemetry.MetricsOtlpEndpoint"];
        }

        private static string GetMetricExporterType()
        {
            return ConfigurationManager.AppSettings["Config.Observability.OpenTelemetry.UseMetricsExporter"].ToUpperInvariant();
        }

        #endregion

        private void DisposeOpenTelemetry()
        {
            this.tracerProvider?.Dispose();
            this.meterProvider?.Dispose();
        }
    }
}
