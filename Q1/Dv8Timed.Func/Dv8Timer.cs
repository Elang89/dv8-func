using System.Globalization;
using CsvHelper;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Dv8Timed.Func
{
    public class Dv8Timer
    {
        private readonly ILogger _logger;

        public Dv8Timer(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Dv8Timer>();
        }

        [Function("Dv8Timer")]
        public void Run([TimerTrigger("0 0 0-23 * * *")] MyInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            string input = "../../rawData.txt";

            string currentDate = DateTimeOffset.Now.ToString("yyyyMMddHHmm");
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            string output = Path.Combine(currentPath, String.Format("../../../data/result-{0}.csv", currentDate));

            Dv8FileWriter fileWriter = new Dv8FileWriter();
            fileWriter.AggregateRecords(input, output);
        }
    }

    public class Dv8FileWriter
    {

        public void AggregateRecords(string input, string output)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;

            string path = Path.Combine(currentPath, input);

            const double kPaConstant = 6.89476;

            string[] lines = System.IO.File.ReadAllLines(input);

            List<WellMovement> movements = File.ReadAllLines(input)
                .Skip(1)
                .ToArray()
                .Select(m => ParseLine(m))
                .Where(m => (m.CHP > 0 && m.CHP < 10000) && (m.WHP > 0 && m.WHP < 10000))
                .GroupBy(m => new { m.WellId })
                .Select(m => new WellMovement
                {
                    WellId = m.Key.WellId,
                    DateTimeStamp = DateTimeOffset.UtcNow,
                    CHP = (m.Average(p => (p.CHP * kPaConstant))),
                    WHP = (m.Average(p => (p.WHP * kPaConstant)))
                })
                .OrderBy(m => m.WellId)
                .ToList();

            WriteNewFile(movements, output);

        }

        public WellMovement ParseLine(string line)
        {
            string[] data = line.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            WellMovement movement = new WellMovement
            {
                WellId = data[0].ToString(),
                DateTimeStamp = DateTimeOffset.Parse(data[1].ToString()),
                WHP = double.Parse(data[2]),
                CHP = double.Parse(data[3])
            };

            return movement;
        }


        public void WriteNewFile(List<WellMovement> movements, string filepath)
        {

            using StreamWriter writer = new StreamWriter(filepath);
            using CsvWriter csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);

            csvWriter.WriteRecords(movements);
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
