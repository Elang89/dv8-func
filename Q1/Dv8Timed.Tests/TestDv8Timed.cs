namespace Dv8Timed.Tests;

using FluentAssertions;
using Dv8Timed.Func;

public class TestDv8Timed
{
    [Fact]
    public void ParseLineShouldReturnWellMovement()
    {
        Dv8FileWriter fileWriter = new Dv8FileWriter();
        string line = "402,2022-10-01T00:02:53,315.87918,88.145836";

        WellMovement movement = fileWriter.ParseLine(line);

        movement.WellId.Should().Be("402");
    }

    [Fact]
    public void WriteFileShouldWriteFile()
    {
        Dv8FileWriter fileWriter = new Dv8FileWriter();

        List<WellMovement> movements = new List<WellMovement>
        {
            new WellMovement{WellId = "300", DateTimeStamp = DateTimeOffset.Now, WHP = 203.412, CHP = 131.42 },
            new WellMovement{WellId = "301", DateTimeStamp = DateTimeOffset.Now, WHP = 89.342, CHP = 102.142 },
            new WellMovement{WellId = "302", DateTimeStamp = DateTimeOffset.Now, WHP = 20.312, CHP = 42.421 }
        };

        string filename = "result.csv";

        fileWriter.WriteNewFile(movements, filename);
        Boolean result = File.Exists(filename);

        result.Should().Be(true);

        File.Delete(filename);

    }
}