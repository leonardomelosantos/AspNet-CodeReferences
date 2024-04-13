using ObservabilitySample.Domain.Service;
using OpenTelemetry.Trace;
using System;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace ObservabilitySample.WebApp.Controllers
{
    /// <summary>
    /// Draft / study.
    /// </summary>
    public class HomeController : Controller
    {
        private Counter<int> _countGreetings;
        private readonly Histogram<int> _metricaHistogram;
        private ActivitySource _greeterActivitySource;

        public HomeController()
        {
            // Metrics
            _countGreetings = CustomMetricsAndActivities.GreeterMeter.CreateCounter<int>("greetings.count", description: "Counts the number of greetings");
            _metricaHistogram = CustomMetricsAndActivities.SegundaMetrica.CreateHistogram<int>("SegundaMetrica.CreateHistogram2", null, "Histogram test");

            // Custom ActivitySource for the application
            _greeterActivitySource = CustomMetricsAndActivities.GreeterActivitySource;
        }

        public ActionResult Index(string id)
        {
            // Create a new Activity scoped to the method
            using (var activity = _greeterActivitySource.StartActivity("My special activity"))
            {
                // METRICS
                _countGreetings.Add(1); // Increment the custom counter

                // Add a tag to the Activity
                activity.DisplayName = "MyBeatifulMethod()";
                activity?.AddTag("testTag", "test");
                activity?.SetTag("greeting", "Hello World!");
                if (DateTime.Now.Second < 30)
                {
                    activity?.SetStatus(Status.Error); // For aleatory tests.
                }
            }
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }
    }
}