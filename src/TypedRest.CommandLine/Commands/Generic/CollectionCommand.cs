using System.Net.Http.Headers;
using TypedRest.Endpoints.Generic;

namespace TypedRest.CommandLine.Commands.Generic;

/// <summary>
/// Command operating on a <see cref="ICollectionEndpoint{TEntity,TElementEndpoint}"/>.
/// </summary>
/// <param name="endpoint">The endpoint this command operates on.</param>
/// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
/// <typeparam name="TEndpoint">The specific type of <see cref="ICollectionEndpoint{TEntity,TElementEndpoint}"/> to operate on.</typeparam>
/// <typeparam name="TElementEndpoint">The specific type of <see cref="IElementEndpoint{TEntity}"/> the <typeparamref name="TEndpoint"/> provides for individual <typeparamref name="TEntity"/>s.</typeparam>
/// <typeparam name="TElementCommand">The specific type of <see cref="IEndpointCommand"/> is used to handle <typeparamref name="TElementEndpoint"/>s. Must have a public constructor with a <typeparamref name="TElementEndpoint"/> parameter.</typeparam>
public abstract class CollectionCommand<TEntity, TEndpoint, TElementEndpoint, TElementCommand>(TEndpoint endpoint) : IndexerCommand<TEndpoint, TElementEndpoint, TElementCommand>(endpoint)
    where TEntity : class
    where TEndpoint : ICollectionEndpoint<TEntity, TElementEndpoint>
    where TElementEndpoint : IElementEndpoint<TEntity>
    where TElementCommand : IEndpointCommand
{
    public override async Task ExecuteAsync(IReadOnlyList<string> args, CancellationToken cancellationToken = default)
    {
        if (args.Count == 0)
        {
            OutputEntities(await Endpoint.ReadAllAsync(cancellationToken));
            return;
        }

        if (GetRange(args[0]) is {} range)
        {
            var elements = await Endpoint.ReadRangeAsync(range, cancellationToken);
            OutputEntities(elements.Elements);
            return;
        }

        switch (args[0].ToLowerInvariant())
        {
            case "create":
                var newEntity = InputEntity(args.Skip(1).ToList());
                if (await Endpoint.CreateAsync(newEntity, cancellationToken) is {} newEndpoint)
                    await GetElementCommand(newEndpoint).ExecuteAsync([], cancellationToken);
                return;

            case "create-all":
                await Endpoint.CreateAllAsync(InputEntities(args.Skip(1).ToList()), cancellationToken);
                return;

            case "set-all":
                await Endpoint.SetAllAsync(InputEntities(args.Skip(1).ToList()), cancellationToken);
                return;
        }

        await base.ExecuteAsync(args, cancellationToken);
    }

    private static RangeItemHeaderValue? GetRange(string input)
    {
        var parts = input.Split('-');
        if (parts.Length != 2) return null;

        long? from = null, to = null;
        if (long.TryParse(parts[0], out long fromOut)) from = fromOut;
        if (long.TryParse(parts[1], out long toOut)) to = toOut;
        return new(from, to);
    }

    /// <summary>
    /// Acquires a <typeparamref name="TEntity"/> from the user, e.g. by parsing the <paramref name="args"/> or via JSON on the console.
    /// </summary>
    protected virtual TEntity InputEntity(IReadOnlyList<string> args)
        => Input<TEntity>(args);

    /// <summary>
    /// Acquires a <typeparamref name="TEntity"/> from the user, e.g. by parsing the <paramref name="args"/> or via JSON on the console.
    /// </summary>
    protected virtual IEnumerable<TEntity> InputEntities(IReadOnlyList<string> args)
        => Input<List<TEntity>>(args);

    /// <summary>
    /// Outputs a collection of <typeparamref name="TEntity"/>s to the user, e.g., via <see cref="object.ToString"/> on the console.
    /// </summary>
    protected virtual void OutputEntities(IEnumerable<TEntity> entities)
    {
        foreach (var entity in entities)
            Console.Write(entity);
    }
}
