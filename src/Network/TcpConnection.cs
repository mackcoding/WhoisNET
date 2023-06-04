using System.Net.Sockets;
using System.Text;

namespace WhoisNET.Network
{
    public class TcpConnection : IDisposable
    {
        private TcpClient Client = new TcpClient();
        private string? _address;
        private int _port;
        private bool disposedValue;

        #region Connection Handling
        public TcpConnection(string ServerAddress, int Port = 43)
        {
            _address = ServerAddress ?? string.Empty;
            _port = Port;

            Debug.WriteDebug($"TcpConnection ready to connect - {_address}:{_port}");

            if (_address == null)
            {
                ThrowException("ServerAddress is null.");
                return;
            }
        }

        public bool Connect()
        {
            try
            {
                Client = new TcpClient(_address ?? string.Empty, _port);
                Debug.WriteDebug($"Connected to {_address}:{_port}, connection successful: {Client.Connected}");
                return true;
            }
            catch (Exception ex)
            {
                ThrowException(ex.Message);
                return false;
            }
        }

        public void Close()
        {
            if (Client != null)
            {
                Debug.WriteDebug($"Connection to {_address}:{_port} has been closed.");
                Client.Close();
            }
            else
                ThrowException("TcpClient is not initialized.");

        }

        public bool IsConnected => Client?.Connected ?? false;

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

        #region Sending/Receiving
        public void Send(string Msg)
        {
            var Data = Encoding.ASCII.GetBytes($"{Msg}\r\n");
            Debug.WriteDebug($"Sending command '{Msg}' to server...");
            CheckIfClientIsValid();

            try
            {
                var Stream = Client.GetStream();
                Stream.Write(Data, 0, Data.Length);

            }
            catch (Exception ex)
            {
                ThrowException(ex.Message);
            }
        }

        public string Receive()
        {
            CheckIfClientIsValid();
            Debug.WriteDebug("Receiving data...");

            try
            {
                NetworkStream Stream = Client.GetStream();
                byte[] Buffer = new byte[1024];
                int BytesRead;
                StringBuilder Received = new StringBuilder();

                while ((BytesRead = Stream.Read(Buffer, 0, Buffer.Length)) > 0)
                {
                    Received.Append(Encoding.ASCII.GetString(Buffer, 0, BytesRead));
                    Debug.WriteDebug($"Received {BytesRead} bytes...");
                }

                Debug.WriteDebug("Receive data complete!");

                return Received.ToString();

            }
            catch (Exception ex)
            {
                ThrowException(ex.Message);
                return string.Empty;
            }

        }
        #endregion

        #region Anything else
        void CheckIfClientIsValid()
        {
            if (Client == null || !Client.Connected)
                ThrowException("There are no active connections.");
        }

        void ThrowException(string Msg)
        {
            Debug.Write(Msg);
            throw new Exception(Msg);
        }
        #endregion
    }
}
