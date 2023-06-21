using System.Runtime.CompilerServices;

namespace Tederean.Apius.Extensions
{

  public static class AsyncExtensions
  {

    public static CancellationTokenAwaiter GetAwaiter(this CancellationToken cancellationToken)
    {
      return new CancellationTokenAwaiter
      {
        _cancellationToken = cancellationToken
      };
    }


    public struct CancellationTokenAwaiter : INotifyCompletion, ICriticalNotifyCompletion
    {

      internal CancellationToken _cancellationToken;


      public bool IsCompleted => _cancellationToken.IsCancellationRequested;


      public CancellationTokenAwaiter(CancellationToken cancellationToken)
      {
        _cancellationToken = cancellationToken;
      }


      public object GetResult()
      {
        if (IsCompleted) throw new OperationCanceledException();

        else throw new InvalidOperationException("The cancellation token has not yet been cancelled.");
      }

      public void OnCompleted(Action continuation)
      {
        _cancellationToken.Register(continuation);
      }

      public void UnsafeOnCompleted(Action continuation)
      {
        _cancellationToken.Register(continuation);
      }
    }
  }
}