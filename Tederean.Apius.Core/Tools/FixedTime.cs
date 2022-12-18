namespace Tederean.Apius.Tools
{

  public class FixedTime : IAsyncDisposable
  {

    private readonly TimeSpan Start;

    private readonly TimeSpan Period;


    public FixedTime(TimeSpan period)
    {
      Start = DateTimeOffset.UtcNow.Offset;
      Period = period;
    }


    public async ValueTask DisposeAsync()
    {
      var end = DateTimeOffset.UtcNow.Offset;
      var elapsedTime = end - Start;

      await Task.Delay(Period - elapsedTime);
    }
  }
}