namespace PowLogger
{
  public class AppConfig
  {
    public string PowDataUrl { get; set; }
    public string CurrentDevice { get; set; }
    public string PowName { get; set; }
    public int CollectionInterval { get; set; }
    public int CooldownSec { get; set; }
    public string InfluxDbServer { get; set; }
    public string InfluxDbDatabase { get; set; }
    public string HostName { get; set; }
  }
}
