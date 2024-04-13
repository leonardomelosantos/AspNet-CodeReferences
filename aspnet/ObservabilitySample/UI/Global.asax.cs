using ObservabilitySample.Domain.Service;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Instrumentation.AspNet;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics;
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
            string jaegerNameIdentifier = "jaeger-all-in-one";
            this.InitializeOpenTelemetryTracer(jaegerNameIdentifier);
            this.InitializeOpenTelemetryMetrics(jaegerNameIdentifier);
        }

        #region Tracer

        private void InitializeOpenTelemetryTracer(string serviceNameIdentifier)
        {
            var builder = Sdk.CreateTracerProviderBuilder()
                .ConfigureResource(r => r.AddService(serviceNameIdentifier)) // It's very important set this service name when using Jaeger/OTLP.
                .AddAspNetInstrumentation(SetAspNetInstrumentationOptions)
                // Put here all Activities names created by you.
                .AddSource(CustomMetricsAndActivities.GreeterActivitySource.Name);

            // If you want OTLP Exporter.
            builder.AddOtlpExporter(); // By configuring using (opts) doesn't work. We need to configure in Environment Variables.

            builder.AddConsoleExporter(options => options.Targets = ConsoleExporterOutputTargets.Debug);

            this.tracerProvider = builder.Build();
        }

        private void SetAspNetInstrumentationOptions(AspNetTraceInstrumentationOptions options)
        {
            options.Enrich = EnrichApsNetInstrumentation;
        }

        private void EnrichApsNetInstrumentation(Activity activity, string arg2, object httpContext)
        {
            if (httpContext is System.Web.HttpRequest request)
            {
                string correctRouteDescription = $"{request.HttpMethod} {request.FilePath}";
                activity.SetTag("http.route", correctRouteDescription);
                activity.DisplayName = correctRouteDescription;
            }
        }

        #endregion

        #region Metrics

        private void InitializeOpenTelemetryMetrics(string theServiceNameIdentifier)
        {
            // Metrics
            // Note: Tracerprovider is needed for metrics to work
            // https://github.com/open-telemetry/opentelemetry-dotnet/issues/2994

            var meterBuilder = Sdk.CreateMeterProviderBuilder()
                .ConfigureResource(r => r.AddService(theServiceNameIdentifier))
                 .AddAspNetInstrumentation()
                 // Put here all Metric names created by you.
                 .AddMeter(CustomMetricsAndActivities.GreeterMeter.Name)
                 .AddMeter(CustomMetricsAndActivities.SegundaMetrica.Name);

            // If you want OTLP Exporter.
            //meterBuilder.AddOtlpExporter(); // By configuring using (opts) doesn't work. We need to configure in Environment Variables.

            meterBuilder.AddPrometheusHttpListener(); // Default: http://localhost:9464/metrics
                                                      // 1) Install Prometheus (default port is 9090 to use UI) and there add http://localhost:9464/metrics in "targets list"
                                                      // 2) Install Graphana from Docker and add Prometheus: http://host.docker.internal:9090

            // If you want console exporter to inspect during the development stage.
            //meterBuilder.AddConsoleExporter((exporterOptions, metricReaderOptions) =>
            //{
            //    exporterOptions.Targets = ConsoleExporterOutputTargets.Debug;
            //});

            this.meterProvider = meterBuilder.Build();
        }

        #endregion

        private void DisposeOpenTelemetry()
        {
            this.tracerProvider?.Dispose();
            this.meterProvider?.Dispose();
        }
    }
}
