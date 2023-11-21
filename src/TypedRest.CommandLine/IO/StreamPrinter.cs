namespace TypedRest.CommandLine.IO;

/// <summary>
/// Prints a stream of <typeparamref name="TEntity"/>s to an <see cref="IConsole"/>.
/// </summary>
/// <param name="console">The console to print to.</param>
public class StreamPrinter<TEntity>(IConsole console) : IObserver<TEntity>
{
    private readonly TaskCompletionSource<bool> _quitEvent = new();

    /// <summary>
    /// Prints all entities provided by the <paramref name="observable"/> to the <see cref="JsonConsole"/>.
    /// </summary>
    /// <remarks>This method is only intended to be called once per class instance.</remarks>
    public async Task PrintAsync(IObservable<TEntity> observable, CancellationToken cancellationToken)
    {
        using (cancellationToken.Register(() => _quitEvent.SetResult(true)))
        using (observable.Subscribe(this))
            await _quitEvent.Task;
    }

    /// <summary>
    /// Outputs a <typeparamref name="TEntity"/> to the user via the console.
    /// </summary>
    public virtual void OnNext(TEntity value)
        => console.Write(value?.ToString());

    /// <summary>
    /// Reprots an <paramref name="error"/> to the user via the console.
    /// </summary>
    public virtual void OnError(Exception error)
    {
        console.WriteError(error.Message);
        _quitEvent.SetResult(true);
    }

    public void OnCompleted() => _quitEvent.SetResult(true);
}
