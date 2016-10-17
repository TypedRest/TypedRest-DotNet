using System;
using System.Threading;
using Nito.AsyncEx;
using TypedRest.CommandLine;
using XProjectNamespaceX.Client.CommandLine.Commands;

namespace XProjectNamespaceX.Client.CommandLine
{
    public static class Program
    {
        public static int Main(string[] args) => AsyncContext.Run(() =>
        {
            var executor = new Executor<MyEntryEndpoint, MyEntryCommand>();
            return executor.RunAsync(args, GetCancellationToken());
        });

        private static CancellationToken GetCancellationToken()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, e) =>
            {
                cancellationTokenSource.Cancel();
                e.Cancel = true;
            };
            return cancellationTokenSource.Token;
        }
    }
}
