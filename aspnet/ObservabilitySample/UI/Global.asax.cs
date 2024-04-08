using OpenTelemetry.Resources;
using OpenTelemetry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using OpenTelemetry.Trace;

namespace ObservabilitySample.WebApp
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private TracerProvider _tracerProvider;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            _tracerProvider = Sdk.CreateTracerProviderBuilder()
                .AddAspNetInstrumentation()

            // Other configuration, like adding an exporter and setting resources
            .AddConsoleExporter()
            .AddSource("my-service-name")
            .SetResourceBuilder(
                ResourceBuilder.CreateDefault()
                    .AddService(serviceName: "my-service-name", serviceVersion: "1.0.0"))
            .Build();
        }

        protected void Application_End()
        {
            _tracerProvider?.Dispose();
        }
    }
}
