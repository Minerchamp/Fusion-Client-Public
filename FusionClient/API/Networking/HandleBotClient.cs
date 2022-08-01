namespace Fusion.Networking
{
    using Fusion.Networking.Serializable;
    using System;
    using System.Net.Sockets;
    using System.Threading.Tasks;

    public class HandleBotClient
    {
        internal TcpClient ClientSocket { get; private set; }

        internal event EventHandler<EventArgs> Connected;

        internal event EventHandler<EventArgs> Disconnected;

        internal event EventHandler<ReceivedPacketEventArgs> ReceivedPacket;

        internal int ClientID { get; private set; }

        internal bool IsConnected;

        internal bool ShouldReconnect = true;

        private NetworkStream clientStream;

        private const int PacketSize = 1024;

        internal void StartClient(TcpClient clientSocket, int clientId)
        {
            ClientID = clientId;
            ClientSocket = clientSocket;
            ClientSocket.SendTimeout = 2000;
            clientStream = ClientSocket.GetStream();
            Task task = new Task(StartThread);
            task.Start();
        }

        internal void Disconnect(bool reconnect = false)
        {
            IsConnected = false;
            ShouldReconnect = reconnect;

            try
            {
                ClientSocket.Client.Close();
            }
            catch { }
        }

        internal void SendHeaderLength(byte[] msg)
        {
            byte[] headerLength = BitConverter.GetBytes(msg.Length);
            try
            {
                clientStream.Write(headerLength, 0, headerLength.Length);
            }
            catch
            {
                Disconnect();
            }
        }

        internal void Send(PacketData packetData)
        {
            var bytes = BsonConverter.ToBson(packetData);

            SendHeaderLength(bytes);

            if (bytes != null && bytes.Length > 0)
            {
                try
                {
                    clientStream.Write(bytes, 0, bytes.Length);
                }
                catch
                {
                    Disconnect();
                }
            }
        }

        private void StartThread()
        {
            Connected?.Invoke(this, new EventArgs());

            IsConnected = true;
            for (; IsConnected;)
            {
                Receive();
            }

            Disconnected?.Invoke(this, new EventArgs());
        }

        private int ReceiveHeaderLength()
        {
            try
            {
                byte[] received = new byte[4];
                _ = clientStream.Read(received, 0, received.Length);
                return BitConverter.ToInt32(received, 0);
            }
            catch
            {
                Disconnect();
                return 0;
            }
        }

        private void Receive()
        {
            int len = ReceiveHeaderLength();

            if (len > 0)
            {
                int remaining = len;
                int totalRead = 0;
                byte[] data = new byte[len];

                int toRead = PacketSize;
                for (; remaining > 0;)
                {
                    toRead = remaining >= PacketSize ? PacketSize : remaining;

                    try
                    {
                        byte[] received = new byte[toRead];
                        int read = clientStream.Read(received, 0, received.Length);

                        totalRead += read;
                        remaining -= read;

                        received.CopyTo(data, totalRead - read);
                    }
                    catch
                    {
                        Disconnect();
                    }
                }

                var packetData = BsonConverter.FromBson<PacketData>(data);
                ReceivedPacket?.Invoke(this, new ReceivedPacketEventArgs(ClientID, packetData));
            }
        }
    }
}