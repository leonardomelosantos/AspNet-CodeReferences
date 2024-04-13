using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace ObservabilitySample.Domain.Service
{
    /// <summary>
    /// Draft / Study
    /// </summary>
    public static class CustomMetricsAndActivities
    {
        // Custom metrics for the application
        public static Meter GreeterMeter = new Meter("OtPrGrYa.Example", "1.0.0");
        public static Meter SegundaMetrica = new Meter("SecondMetric.Example2", "1.0.0");

        // Custom ActivitySource for the application
        public static ActivitySource GreeterActivitySource = new ActivitySource("OtPrGrJa.Example");
    }
}
