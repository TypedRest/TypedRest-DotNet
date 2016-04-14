using System;
using System.Threading;
using System.Threading.Tasks;

namespace TypedRest.CommandLine
{
    /// <summary>
    /// Prints a stream of entities to the <see cref="Console"/>.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class StreamPrinter<TEntity> : IObserver<TEntity>
    {
        private readonly TaskCompletionSource<bool> _quitEvent = new TaskCompletionSource<bool>();

        /// <summary>
        /// Prints all entities provided by the <paramref name="observable"/> to the <see cref="Console"/>.
        /// </summary>
        /// <remarks>This method is only intended to be called once per class instance.</remarks>
        public async Task PrintAsync(IObservable<TEntity> observable, CancellationToken cancellationToken)
        {
            using (cancellationToken.Register(() => _quitEvent.SetResult(true)))
            using (observable.Subscribe(this))
                await _quitEvent.Task;
        }

        public void OnNext(TEntity value)
        {
            Console.WriteLine(value.ToString());
        }

        public void OnError(Exception error)
        {
            Console.Error.WriteLine(error.Message);
            _quitEvent.SetResult(true);
        }

        public void OnCompleted()
        {
            _quitEvent.SetResult(true);
        }
    }
}