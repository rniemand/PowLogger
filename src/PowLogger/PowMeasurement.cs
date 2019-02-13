using System.Text.RegularExpressions;

namespace PowLogger
{
  public class PowMeasurement
  {
    public bool Valid { get; set; }
    public int Voltage { get; set; }
    public double Current { get; set; }
    public int Power { get; set; }
    public double PowerFactor { get; set; }
    public double EnergyToday { get; set; }
    public double EnergyYesterday { get; set; }
    public double EnergyTotal { get; set; }

    public PowMeasurement(string htmlResponse)
    {
      var expression = "voltage{m}(\\d+) V.*?" + // 1 (127 V)
                       "Current{m}([.|\\d]+) A.*?" + // 2 (0.093 A)
                       "Power{m}(\\d+) W.*?" + // 3 (10 W)
                       "Power Factor{m}([.|\\d]+){e}.*?" +// 4 (0.82)
                       "Energy Today{m}([.|\\d]+) kWh.*?" + // 5 (0.000 kWh)
                       "Energy Yesterday{m}([.|\\d]+) kWh.*?" + // 6 (0.399 kWh)
                       "Energy Total{m}([.|\\d]+) kWh"; // 7 (42.498 kWh)

      if (!Regex.IsMatch(htmlResponse, expression, RegexOptions.IgnoreCase))
      {
        Valid = false;
        return;
      }

      var match = Regex.Match(htmlResponse, expression, RegexOptions.IgnoreCase);

      Valid = true;
      Voltage = match.GetInt(1);
      Current = match.GetDouble(2);
      Power = match.GetInt(3);
      PowerFactor = match.GetDouble(4);
      EnergyToday = match.GetDouble(5);
      EnergyYesterday = match.GetDouble(6);
      EnergyTotal = match.GetDouble(7);
    }

    public PowMeasurement()
    {
      Valid = false;
    }
  }
}