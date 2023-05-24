namespace Dv8TimedFunc
{
    public class WellMovement
    {
        public string? WellId { get; set; }
        public DateTimeOffset? DateTimeStamp { get; set; }
        public double WHP { get; set; }
        public double CHP { get; set; }
    }

    public class AggregatedWellMovement
    {
        public string? WellId { get; }
        public DateTimeOffset? DateAdded { get; }
        public double Aggregate;

    }
}