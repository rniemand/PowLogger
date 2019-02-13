using System.Text.RegularExpressions;

namespace PowLogger
{
  public static class RegexExtensions
  {
    public static int GetInt(this Match match, int groupNumber, int fallback = 0)
    {
      if (match.Groups.Count < groupNumber)
        return fallback;

      if (int.TryParse(match.Groups[groupNumber].Value, out int parsed))
      {
        return parsed;
      }

      return fallback;
    }

    public static double GetDouble(this Match match, int groupNumber, double fallback = 0)
    {
      if (match.Groups.Count < groupNumber)
        return fallback;

      if (double.TryParse(match.Groups[groupNumber].Value, out double parsed))
      {
        return parsed;
      }

      return fallback;
    }
  }
}