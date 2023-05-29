using System.Net.Sockets;
using System.Text;

namespace WhoisNET.Network
{
    public class TcpConnection : IDisposable
    {
        private TcpClient Client = new TcpClient();
        private string _address;
        private int _port;
        private bool disposedValue;

        #region Connection Handling
        public TcpConnection(string ServerAddress, int Port = 43)
        {
            _address = ServerAddress ?? string.Empty;
            _port = Port;

            if (_address == null)
            {
                Debug.Error("ServerAddress is null.");
                throw new Exception("ServerAddress is null.");
            }
        }

        public bool Connect()
        {
            try
            {
                Client = new TcpClient(_address, _port);
                Debug.Write($"Connected to {_address}:{_port} \n IsConnected: {Client.Connected}");
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
                Debug.Write($"Connection to {_address}:{_port} has been closed.");
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
            Debug.Write($"Sending command '{Msg}' to server...");

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

            Debug.Write("Receiving data...");

            try
            {
                NetworkStream Stream = Client.GetStream();
                byte[] Buffer = new byte[Client.ReceiveBufferSize];
                int BufferSizeRead = Stream.Read(Buffer, 0, Buffer.Length);
                byte[] Data = new byte[BufferSizeRead];

                Array.Copy(Buffer, Data, BufferSizeRead);

                Debug.Write($"Data received ({BufferSizeRead} bytes)");

                return Encoding.ASCII.GetString(Data);
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
