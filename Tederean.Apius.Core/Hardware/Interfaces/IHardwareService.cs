namespace Tederean.Apius.Hardware
{

  public interface IHardwareService : IDisposable
  {

    IMainboardService? MainboardService { get; }

    IGraphicsCardService? GraphicsCardService { get; }


    HardwareSensors GetHardwareInfo();
  }
}
