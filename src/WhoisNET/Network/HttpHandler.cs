namespace WhoisNET.Network
{
    /// <summary>
    /// Handles HTTP requests and content retrieval.
    /// </summary>
    /// <param name="url">The default URL to use for requests.</param>
    public class HttpHandler(string? url = null) : IAsyncDisposable
    {
        // todo: implement a way to allow the timeout to be changed
        private static readonly HttpClient _sharedClient = new()
        {
            Timeout = TimeSpan.FromSeconds(30),

        };
        private bool _disposed;

        /// <summary>
        /// Reads the entire content from the specified URI asynchronously.
        /// </summary>
        /// <param name="requestUri">The URI to request content from. If null, uses the default URL.</param>
        /// <returns>A string containing the entire content.</returns>
        public async Task<string> GetAllContentAsync(string? requestUri = null)
        {
            ThrowIfDisposed();
            using var stream = await GetStreamAsync(requestUri).ConfigureAwait(false);
            using var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Reads the content from the specified URI line by line asynchronously.
        /// </summary>
        /// <param name="requestUri">The URI to request content from. If null, uses the default URL.</param>
        /// <returns>An asynchronous enumerable of strings, each representing a line of content.</returns>
        public async IAsyncEnumerable<string> GetContentByLineAsync(string? requestUri = null)
        {
            ThrowIfDisposed();
            using var stream = await GetStreamAsync(requestUri).ConfigureAwait(false);
            using var reader = new StreamReader(stream);
            string? line;
            while ((line = await reader.ReadLineAsync().ConfigureAwait(false)) is not null)
            {
                yield return line;
            }
        }

        /// <summary>
        /// Gets the HTTP network stream from the specified URI.
        /// </summary>
        /// <param name="requestUri">The URI to request the stream from. If null, uses the default URL.</param>
        /// <returns>A Stream object representing the HTTP response content.</returns>
        private async Task<Stream> GetStreamAsync(string? requestUri = null)
        {
            try
            {
                return await _sharedClient.GetStreamAsync(requestUri ?? url ??
                    throw new ArgumentNullException(nameof(requestUri))).ConfigureAwait(false);
            }
            catch (HttpRequestException ex)
            {
                Debug.ThrowException($"HTTP request failed: {ex.Message}", exception: ex);
                throw;
            }
        }

        /// <summary>
        /// Asynchronously releases the unmanaged resources used by the HttpHandler.
        /// </summary>
        public ValueTask DisposeAsync()
        {
            _disposed = true;
            GC.SuppressFinalize(this);
            return ValueTask.CompletedTask;
        }

        /// <summary>
        /// Throws an exception is the object is disposed.
        /// </summary>
        private void ThrowIfDisposed()
        {
            ObjectDisposedException.ThrowIf(_disposed, nameof(HttpHandler));
        }
    }
}
