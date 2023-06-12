using System.Net.Sockets;
using System.Text;

namespace WhoisNET.Network
{
    public class TcpConnection : IDisposable
    {
        private TcpClient Client = new();
        private string? _address;
        private int _port;
        private bool disposedValue;

        #region Connection Handling
        /// <summary>
        /// Initializes TCPCOnnection with specific ServerAddress/port (default 43)
        /// </summary>
        /// <param name="ServerAddress">The address we are attempting connect </param>
        /// <param name="Port">Port we are connecting to; default: 43</param>
        public TcpConnection(string ServerAddress, int Port = 43)
        {
            _address = ServerAddress ?? string.Empty;
            _port = Port;

            Debug.WriteDebug($"TcpConnection ready to connect - {_address}:{_port}");

            if (string.IsNullOrEmpty(_address))
            {
                Debug.WriteError("ServerAddress is null.");
                throw new Exception("ServerAddress parameter is null.");
            }
        }

        /// <summary>
        /// Connects to the server
        /// </summary>
        /// <returns>True if connected/false if not connected</returns>
        public bool Connect()
        {
            if (string.IsNullOrEmpty(_address)) return false;

            try
            {
                Client = new TcpClient(_address ?? string.Empty, _port);
                Debug.WriteDebug($"Connected to {_address}:{_port}, connection successful: {Client.Connected}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteError(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Connects to the server async
        /// </summary>
        /// <returns>True if connected/false if not connected</returns>
        public async Task<bool> ConnectAsync()
        {
            if (string.IsNullOrEmpty(_address)) return false;

            try
            {
                Client = new TcpClient();
                Task connectTask = Client.ConnectAsync(_address ?? string.Empty, _port);
                await connectTask.ConfigureAwait(false);

                Debug.WriteDebug($"Connected to {_address}:{_port}, connection successful: {Client.Connected}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteError(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Closes all the TCP connections
        /// </summary>
        public void Close()
        {
            if (Client != null)
            {
                Debug.WriteDebug($"Connection to {_address}:{_port} has been closed.");
                Client.Close();
            }
            else
                Debug.WriteError("TcpClient is not initialized.");

        }

        #endregion

        #region Async

        public async Task SendAsync(string Msg)
        {
            if (string.IsNullOrEmpty(_address)) return;

            var Data = Encoding.ASCII.GetBytes($"{Msg}\r\n");
            Debug.WriteDebug($"Sending command '{Msg}' to server....");
            CheckIfClientIsValid();

            try
            {
                var Stream = Client.GetStream();
                await Stream.WriteAsync(Data, 0, Data.Length);
            }
            catch (Exception ex)
            {
                Debug.WriteError(ex.Message);
            }
        }

        public async Task<string> ReceiveAsync()
        {
            if (string.IsNullOrEmpty(_address)) return string.Empty;
            CheckIfClientIsValid();
            Debug.WriteDebug("Receiving data...");

            try
            {
                NetworkStream Stream = Client.GetStream();
                byte[] Buffer = new byte[1024];
                int BytesRead;
                StringBuilder Received = new();

                while ((BytesRead = await Stream.ReadAsync(Buffer, 0, Buffer.Length)) > 0)
                {
                    Received.Append(Encoding.ASCII.GetString(Buffer, 0, BytesRead));
                    Debug.WriteDebug($"Received {BytesRead} bytes...");
                }

                Debug.WriteDebug($"Receive data complete.");

                return Received.ToString();
            }
            catch (Exception ex)
            {
                Debug.WriteError(ex.Message);
                return string.Empty;
            }
        }

        #endregion

        #region Sync


        public void Send(string Msg)
        {
            if (string.IsNullOrEmpty(_address)) return;

            var Data = Encoding.ASCII.GetBytes($"{Msg}\r\n");
            Debug.WriteDebug($"Sending command '{Msg}' to server....");
            CheckIfClientIsValid();

            try
            {
                var Stream = Client.GetStream();
                Stream.Write(Data, 0, Data.Length);
            }
            catch (Exception ex)
            {
                Debug.WriteError(ex.Message);
            }
        }

        public string Receive()
        {
            if (string.IsNullOrEmpty(_address)) return string.Empty;
            CheckIfClientIsValid();
            Debug.WriteDebug("Receiving data...");

            try
            {
                NetworkStream Stream = Client.GetStream();
                byte[] Buffer = new byte[1024];
                int BytesRead;
                StringBuilder Received = new();

                while ((BytesRead = Stream.Read(Buffer, 0, Buffer.Length)) > 0)
                {
                    Received.Append(Encoding.ASCII.GetString(Buffer, 0, BytesRead));
                    Debug.WriteDebug($"Received {BytesRead} bytes...");
                }


                Debug.WriteDebug($"Receive data complete.");

                return Received.ToString();

            }
            catch (Exception ex)
            {
                Debug.WriteError(ex.Message);
                return string.Empty;
            }

        }

        public bool IsConnected => Client?.Connected ?? false;


        #endregion

        #region Anything else
        void CheckIfClientIsValid()
        {
            if (Client == null || !Client.Connected)
            {
                Debug.WriteError("There are no active connections.");
                //throw new Exception("There are no active connections.");
            }
        }



        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Close();
                    Client.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
