using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using InfluxDB.Collector;

namespace PowLogger
{
  class Program
  {
    private static HttpClient _httpClient;
    private static long _submitCount = 0;
    private static bool _inCooldown;
    private static DateTime? _cooldownEndTime;

    static void Main(string[] args)
    {
      ConfigureInflux();
      ConfigureHttpClient();

      // Log some useful information before we start
      Log.Info($"Setting POW Data URL to: {Config.AppConfig.PowDataUrl}");
      Log.Info($"Logging data for current role: {Config.AppConfig.CurrentDevice}");
      Log.Info($"Logging data as: {Config.AppConfig.PowName}");
      Log.Info("Starting data collection loop");

      // Run the main collection loop
      for (; ; )
      {
        // If "HandleCooldown" return TRUE we are good to collect again
        if (HandleCooldown())
        {
          var measurement = GetLatestMeasurement();

          if (!measurement.Valid)
            Console.WriteLine("Invalid measurement, dropping it");
          else
            SubmitMeasurement(measurement);
        }

        // Wait a second before collecting again
        Thread.Sleep(Config.AppConfig.CollectionInterval);
      }
    }

    private static void ConfigureInflux()
    {
      Log.Info("Configuring InfluxDB connection");

      // Set the desired host name
      var hostName = Config.AppConfig.HostName.ToLower().Trim();
      if (hostName == "{host}")
        hostName = Environment.MachineName.ToLower().Trim();
      Log.Debug($"Setting hostname to: {hostName}");

      // Create a new instance of the MetricsCollector
      Metrics.Collector = new CollectorConfiguration()
        .Tag.With("host", hostName)
        .Batch.AtInterval(TimeSpan.FromSeconds(2))
        .WriteTo.InfluxDB(Config.AppConfig.InfluxDbServer, Config.AppConfig.InfluxDbDatabase)
        .CreateCollector();

      // http://10.0.0.50:8888/sources/2/chronograf/data-explorer
      // CREATE DATABASE "data"
      // DROP DATABASE "data"
    }

    private static void ConfigureHttpClient()
    {
      Console.WriteLine("Creating HttpClient for data collection");

      _httpClient = new HttpClient();
    }

    private static PowMeasurement GetLatestMeasurement()
    {
      try
      {
        using (var request = new HttpRequestMessage(HttpMethod.Get, Config.AppConfig.PowDataUrl))
        {
          var response = _httpClient.SendAsync(request)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

          var body = response.Content.ReadAsStringAsync()
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

          return new PowMeasurement(body);
        }
      }
      catch (Exception ex)
      {
        Log.Error($"Unable to collect data: {ex.GetType().Name}: {ex.Message}");

        _inCooldown = true;
        _cooldownEndTime = DateTime.Now.AddSeconds(Config.AppConfig.CooldownSec);
        Log.Info($"In cooldown until: {_cooldownEndTime}");

        return new PowMeasurement();
      }
    }

    private static bool HandleCooldown()
    {
      // Not in a cooldown - can collect data
      if (!_inCooldown)
        return true;

      // Has not been long enough, no collection for you
      if (_cooldownEndTime > DateTime.Now)
        return false;

      // Cooldown has ended - we are good to go
      _inCooldown = false;
      _cooldownEndTime = null;
      Log.Info("Cooldown has ended attempting to collect data again");

      return true;
    }

    private static void SubmitMeasurement(PowMeasurement measurement)
    {
      Metrics.Write("sonoff_pow",
        new Dictionary<string, object>
        {
          {"Voltage", measurement.Voltage},
          {"Current", measurement.Current},
          {"Power", measurement.Power},
          {"PowerFactor", measurement.PowerFactor},
          {"EnergyToday", measurement.EnergyToday},
          {"EnergyYesterday", measurement.EnergyYesterday},
          {"EnergyTotal", measurement.EnergyTotal}
        },
        new Dictionary<string, string>
        {
          {"Role", Config.AppConfig.CurrentDevice},
          {"DeviceName", Config.AppConfig.PowName}
        });

      // Keep track of submitted events
      _submitCount++;
      if (_submitCount % 100 == 0)
        Console.WriteLine($"Submitted {_submitCount} data points to InfluxDB");
    }
  }
}
