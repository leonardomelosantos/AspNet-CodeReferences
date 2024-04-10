using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ObservabilitySample.Domain.Service;
using Microsoft.Extensions.Logging;

namespace ObservabilitySample.WebApp.Controllers
{
    /// <summary>
    /// Draft / study.
    /// </summary>
    public class HomeController : Controller
    {
        private Counter<int> _countGreetings;
        private ActivitySource _greeterActivitySource;

        public HomeController()
        {
            // Metrics
            _countGreetings = CustomMetricsAndActivities.GreeterMeter.CreateCounter<int>("greetings.count", description: "Counts the number of greetings");
            // Custom ActivitySource for the application
            _greeterActivitySource = CustomMetricsAndActivities.GreeterActivitySource;
        }

        public ActionResult Index()
        {
            // Create a new Activity scoped to the method
            using (var activity = _greeterActivitySource.StartActivity("GreeterActivity"))
            {
                // Log a message
                //logger.LogInformation("Sending greeting");

                // Increment the custom counter
                _countGreetings.Add(1);

                // Add a tag to the Activity
                activity?.SetTag("greeting", "Hello World!");
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