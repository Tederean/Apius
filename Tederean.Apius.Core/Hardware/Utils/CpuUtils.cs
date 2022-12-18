using System.Text;

namespace Tederean.Apius.Hardware
{

  public static class CpuUtils
  {

    public static string GetCpuName(string cpuBrandName)
    {
      var stringBuilder = new StringBuilder(cpuBrandName);

      stringBuilder.Replace("(R)", string.Empty);
      stringBuilder.Replace("(TM)", string.Empty);
      stringBuilder.Replace("(tm)", string.Empty);
      stringBuilder.Replace("CPU", string.Empty);
      stringBuilder.Replace("Dual-Core Processor", string.Empty);
      stringBuilder.Replace("Triple-Core Processor", string.Empty);
      stringBuilder.Replace("Quad-Core Processor", string.Empty);
      stringBuilder.Replace("Six-Core Processor", string.Empty);
      stringBuilder.Replace("Eight-Core Processor", string.Empty);
      stringBuilder.Replace("6-Core Processor", string.Empty);
      stringBuilder.Replace("8-Core Processor", string.Empty);
      stringBuilder.Replace("12-Core Processor", string.Empty);
      stringBuilder.Replace("16-Core Processor", string.Empty);
      stringBuilder.Replace("24-Core Processor", string.Empty);
      stringBuilder.Replace("32-Core Processor", string.Empty);
      stringBuilder.Replace("64-Core Processor", string.Empty);

      foreach (var _ in Enumerable.Range(0, 10))
        stringBuilder.Replace("  ", " ");

      var cpuName = stringBuilder.ToString();

      if (cpuName.Contains("@"))
        cpuName = cpuName.Remove(cpuName.IndexOf("@"));

      return cpuName.Trim();
    }

    public static CpuVendor GetCpuVendor(string vendorId)
    {
      return vendorId switch
      {
        "GenuineIntel" => CpuVendor.Intel,
        "AuthenticAMD" => CpuVendor.AMD,
        _ => CpuVendor.Unknown,
      };
    }
  }
}
