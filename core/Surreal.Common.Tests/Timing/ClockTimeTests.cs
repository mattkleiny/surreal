namespace Surreal.Timing;

public class ClockTimeTests
{
  [Test]
  [TestCase("00:00:00")]
  [TestCase("12:30:00")]
  [TestCase("23:59:59")]
  public void it_should_parse_simple_time_representations(string raw)
  {
    var clockTime = ClockTime.Parse(raw);

    clockTime.ToString().Should().Be(raw);
  }
}
