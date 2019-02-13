using System;

namespace PowLogger
{
  public static class Log
  {
    public static void Debug(string message)
    {
      Console.WriteLine($"{GetDate()} [DEBUG] {message}");
    }

    public static void Info(string message)
    {
      Console.WriteLine($"{GetDate()} [INFO] {message}");
    }

    public static void Warning(string message)
    {
      Console.WriteLine($"{GetDate()} [WARNING] {message}");
    }

    public static void Error(string message)
    {
      Console.WriteLine($"{GetDate()} [ERROR] {message}");
    }

    // Internal methods
    private static string GetDate()
    {
      var now = DateTime.Now;

      return $"{now.Year}-" +
             $"{now.Month.ToString().PadLeft(2, '0')}-" +
             $"{now.Day.ToString().PadLeft(2, '0')} " +
             $"{now.Hour.ToString().PadLeft(2, '0')}:" +
             $"{now.Minute.ToString().PadLeft(2, '0')}:" +
             $"{now.Second.ToString().PadLeft(2, '0')}";
    }
  }
}
