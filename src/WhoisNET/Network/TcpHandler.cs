using System.Net.Sockets;
using System.Text;

namespace WhoisNET.Network
{
    public class TcpHandler(string hostname = "whois.iana.org", int queryPort = 43) : IAsyncDisposable
    {
        private readonly TcpClient _client = new();
        private NetworkStream? _stream;

        public async Task ConnectAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                Debug.WriteDebug($"Connecting to {hostname}:{queryPort}.");
                await _client.ConnectAsync(hostname, queryPort, cancellationToken).ConfigureAwait(false);
                _stream = _client.GetStream();
            }
            catch (SocketException ex)
            {
                Debug.ThrowException(ex.Message, exception: ex);
                throw;
            }
        }

        public async Task WriteAsync(string data, CancellationToken cancellationToken = default)
        {
            if (!_client.Connected)
                await ConnectAsync(cancellationToken).ConfigureAwait(false);

            if (_stream is null)
                throw new InvalidOperationException($"{nameof(_stream)} is null.");

            try
            {
                var buffer = Encoding.ASCII.GetBytes($"{data}\r\n");
                await _stream.WriteAsync(buffer, cancellationToken).ConfigureAwait(false);
                Debug.WriteDebug($"Wrote data '{data}\\r\\n'");
            }
            catch (ObjectDisposedException ex)
            {
                Debug.ThrowException($"Stream is closed: {ex.Message}", exception: ex);
                throw;
            }
            catch (IOException ex)
            {
                Debug.ThrowException($"Error writing to stream: {ex.Message}", exception: ex);
                throw;
            }
        }

        public async Task<string> ReadAsync(CancellationToken cancellationToken = default)
        {
            if (!_client.Connected)
                await ConnectAsync(cancellationToken).ConfigureAwait(false);

            if (_stream is null)
                throw new InvalidOperationException($"{nameof(_stream)} is null.");

            try
            {
                using var memoryStream = new MemoryStream();
                var buffer = new byte[1024];
                int bytesRead;

                while ((bytesRead = await _stream.ReadAsync(buffer, cancellationToken).ConfigureAwait(false)) > 0)
                    await memoryStream.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken).ConfigureAwait(false);

                memoryStream.Position = 0;
                using var streamReader = new StreamReader(memoryStream);
                return await streamReader.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (ObjectDisposedException ex)
            {
                Debug.ThrowException($"Stream is closed: {ex.Message}", exception: ex);
                throw;
            }
            catch (IOException ex) when (ex.InnerException is SocketException)
            {
                Debug.ThrowException($"Error reading from stream: {ex.Message}", exception: ex);
                throw;
            }
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore().ConfigureAwait(false);
            GC.SuppressFinalize(this);
        }

        protected virtual async ValueTask DisposeAsyncCore()
        {
            if (_stream != null)
            {
                await _stream.DisposeAsync().ConfigureAwait(false);
            }
            _client.Dispose();
        }
    }
}
