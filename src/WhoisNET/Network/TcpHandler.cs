using System.Net.Sockets;
using System.Text;

namespace WhoisNET.Network
{
    public class TcpHandler(string Hostname = "whois.iana.org", int QueryPort = 43) : IDisposable
    {
        private readonly TcpClient _client = new();
        private NetworkStream? _stream;

        public async Task ConnectAsync()
        {
            Hostname ??= "whois.iana.org";

            try
            {
                Debug.WriteDebug($"Connecting to {Hostname}:{QueryPort}.");
                await _client.ConnectAsync(Hostname, QueryPort);
                _stream = _client.GetStream();
            }
            catch (SocketException ex)
            {
                Debug.ThrowException($"{ex.Message}", exception: ex);
                throw;
            }
        }

        public async Task WriteAsync(string data)
        {
            if (!_client.Connected)
                await ConnectAsync();

            // todo: is this needed? currently fixes dereference warning
            if (_stream is null)
            {
                Debug.ThrowException($"The NetworkStream is null.");
                return;
            }

            // todo: needs major improvements and/or overhaul.
            try
            {
                var buffer = Encoding.ASCII.GetBytes($"{data}\r\n");
                await _stream.WriteAsync(buffer);
                Debug.WriteDebug($"Wrote data '{data}\\r\\n'");
            }
            catch (ObjectDisposedException ex)
            {
                Debug.ThrowException($"Stream is closed, exception: {ex.Message}", exception:ex);
                throw;
            }
            catch (IOException ex)
            {
                Debug.ThrowException($"Error writing to stream, exception: {ex.Message}");
                throw;
            }
        }


        public async Task<string> ReadAsync()
        {
            if (!_client.Connected)
                await ConnectAsync();

            // todo: is this needed? currently fixes dereference warning
            if (_stream is null)
            {
                Debug.ThrowException($"The network stream is null.");
                return string.Empty;
            }

            try
            {
                using var memoryStream = new MemoryStream();
                var buffer = new byte[1024];
                int bytesRead;

                while ((bytesRead = await _stream.ReadAsync(buffer)) > 0)
                    await memoryStream.WriteAsync(buffer.AsMemory(0, bytesRead));

                memoryStream.Position = 0;
                using var streamReader = new StreamReader(memoryStream);
                return await streamReader.ReadToEndAsync();
            }
            catch (ObjectDisposedException ex)
            {
                Debug.ThrowException($"Stream is closed, exception: {ex.Message}",exception:ex);
                throw;
            }
            catch (IOException ex) when (ex.InnerException is SocketException)
            {
                Debug.ThrowException($"Error reading from stream, exception: {ex.Message}", exception:ex);
                throw;
            }
        }




        protected virtual void Dispose(bool disposing)
        {
            if (disposing && _client != null)
            {
                _stream?.Dispose();
                _client.Close();
                _client.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~TcpHandler()
        {
            Dispose(false);
        }
    }
}
