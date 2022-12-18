namespace Tederean.Apius.Hardware
{

  public interface IGraphicsCardService : IDisposable
  {

    string GraphicsCardName { get; }


    GraphicsCardSensors GetGraphicsCardSensors();
  }
}
