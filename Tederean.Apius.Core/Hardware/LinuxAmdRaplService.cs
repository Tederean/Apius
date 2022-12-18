#if LINUX
using System.Diagnostics;

namespace Tederean.Apius.Hardware
{

  public class LinuxAmdRaplService : IRaplService
  {

    private readonly Dictionary<MsrValue, long> msrValues = new Dictionary<MsrValue, long>()
    {
      [MsrValue.PowerUnit] = 0xC0010299L,
      [MsrValue.CoreEnergy] = 0xC001029AL,
      [MsrValue.PackageEnergy] = 0xC001029BL,
    };

    private readonly PackageRaplData[] _packagesRaplData;


    private PackageEnergySample[]? _lastPackageEnergySamples;


    public LinuxAmdRaplService(int[] virtualCores)
    {
      var packageCores = virtualCores.Select(coreId =>
      {
        var packageIdPath = $"/sys/devices/system/cpu/cpu{coreId}/topology/physical_package_id";
        var packageIdContent = File.ReadAllText(packageIdPath);

        return (CoreId: coreId, PackageId: int.Parse(packageIdContent));

      }).GroupBy(x => x.PackageId).Select(group => group.First().CoreId).ToArray();


      _packagesRaplData = packageCores.Select(packageCore =>
      {
        var msrFilePath = $"/dev/cpu/{packageCore}/msr";

        using var msrFileStream = File.OpenRead(msrFilePath);

        (var _, var value) = ReadMsr(msrFileStream, MsrValue.PowerUnit);

        var energyFactor = Math.Pow(0.5, (value & 0x1F00UL) >> 8);

        return new PackageRaplData(msrFilePath, energyFactor);

      }).ToArray();
    }

    public void Dispose()
    {
    }


    public void GetWattageValues(MainboardSensors mainboardSensors)
    {
      var lastPackageEnergySamples = _lastPackageEnergySamples;
      var currentPackageEnergySamples = _lastPackageEnergySamples = ReadPackageEnergy();


      if (lastPackageEnergySamples != null)
      {
        var currentWattage_W = Enumerable.Range(0, _packagesRaplData.Length).Select(index =>
        {
          var lastPackageEnergySample = lastPackageEnergySamples[index];
          var currentPackageEnergySample = currentPackageEnergySamples[index];

          var elapsedTime_sec = Math.Abs(currentPackageEnergySample.Timestamp - lastPackageEnergySample.Timestamp) / (double)Stopwatch.Frequency;
          var usedEnergy_J = Math.Abs(currentPackageEnergySample.Energy_J - lastPackageEnergySample.Energy_J);

          return usedEnergy_J / elapsedTime_sec;

        }).Sum();

        mainboardSensors.Wattage_W = new Sensor(currentWattage_W, 0.0, 105.0);
      }
    }


    private PackageEnergySample[] ReadPackageEnergy()
    {
      return _packagesRaplData.Select(packageRaplData =>
      {
        using var msrFileStream = File.OpenRead(packageRaplData.MsrFilePath);

        (var timestamp, var value) = ReadMsr(msrFileStream, MsrValue.PackageEnergy);

        return new PackageEnergySample(value * packageRaplData.EnergyUnitFactor, timestamp);

      }).ToArray();
    }

    private (long elapsedTicks, ulong value) ReadMsr(FileStream msrPackageStream, MsrValue msrValue)
    {
      msrPackageStream.Seek(msrValues[msrValue], SeekOrigin.Begin);

      var buffer = new byte[8];

      msrPackageStream.Read(buffer, offset: 0, buffer.Length);

      var elapsedTicks = Stopwatch.GetTimestamp();
      var value = BitConverter.ToUInt64(buffer, startIndex: 0);

      return (elapsedTicks, value);
    }



    private record class PackageRaplData(string MsrFilePath, double EnergyUnitFactor);


    private record class PackageEnergySample(double Energy_J, long Timestamp);


    private enum MsrValue
    {
      PowerUnit,
      CoreEnergy,
      PackageEnergy,
    }
  }
}
#endif