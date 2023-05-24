using System;
using System.Reflection;
using System.Text;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Dv8TimedFunc
{
    public class Dv8Timer
    {
        private readonly ILogger _logger;

        public Dv8Timer(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Dv8Timer>();
        }

        [Function("Dv8Timer")]
        public void Run([TimerTrigger("*/15 * * * * *")] MyInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            // _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");

            OpenFile();
        }

        private void OpenFile()
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            string filename = "../../../rawData.txt";
            string path = Path.Combine(currentPath, filename);
            List<WellMovement> movements = new List<WellMovement>();

            using FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None);
            using StreamReader sr = new StreamReader(fs);

            string? line;
            string? header = sr.ReadLine();

            while ((line = sr.ReadLine()) != null)
            {
                WellMovement movement = ParseLine(line);
                movements.Append(movement);
            }

        }

        private WellMovement ParseLine(string line)
        {
            string[] data = line.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            WellMovement movement = new WellMovement
            {
                WellId = data[0].ToString(),
                DateTimeStamp = DateTimeOffset.Parse(data[1].ToString()),
                WHP = double.Parse(data[2].ToString()),
                CHP = double.Parse(data[3])
            };

            return movement;
        }

        private void filterAndAggregate(List<WellMovement> movements)
        {
            List<AggregatedWellMovement> aggregated = new List<AggregatedWellMovement>();
            var filtered = movements
                .Where(m => (m.CHP > 0 && m.CHP < 10000) && (m.WHP > 0 && m.WHP < 10000));
        }
    }


    public class MyInfo
    {
        public MyScheduleStatus ScheduleStatus { get; set; }

        public bool IsPastDue { get; set; }
    }

    public class MyScheduleStatus
    {
        public DateTime Last { get; set; }

        public DateTime Next { get; set; }

        public DateTime LastUpdated { get; set; }
    }


}
