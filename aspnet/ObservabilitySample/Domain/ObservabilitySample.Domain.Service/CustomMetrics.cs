using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ObservabilitySample.Domain.Service
{
    public static class CustomMetrics
    {
        // Custom metrics for the application
        public static Meter greeterMeter = new Meter("OtPrGrYa.Example", "1.0.0");

        // Custom ActivitySource for the application
        public static ActivitySource greeterActivitySource = new ActivitySource("OtPrGrJa.Example");
    }
}
