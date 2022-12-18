namespace Tederean.Apius.Hardware
{

  public interface IRaplService : IDisposable
  {

    void GetWattageValues(MainboardSensors mainboardValues);
  }
}
