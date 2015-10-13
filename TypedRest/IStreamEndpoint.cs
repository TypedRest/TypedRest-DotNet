using System;

namespace TypedRest
{
    /// <summary>
    /// REST endpoint that represents a stream of elements.
    /// </summary>
    /// <typeparam name="TElement">The type of element the endpoint represents.</typeparam>
    public interface IStreamEndpoint<TElement> : IPaginationEndpoint<TElement>
    {
        /// <summary>
        /// Provides an observable stream of elements.
        /// </summary>
        /// <param name="startIndex">The index of the first element to return in the stream. Use negative values to start counting from the end of the stream.</param>
        IObservable<TElement> GetStream(long startIndex = 0);
    }
}