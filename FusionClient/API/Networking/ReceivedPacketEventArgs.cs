namespace Fusion.Networking
{
    using Serializable;

    public class ReceivedPacketEventArgs
    {
        public PacketData Data { get; private set; }

        public int ClientID { get; private set; }

        public ReceivedPacketEventArgs(int clientID, PacketData data)
        {
            Data = data;
            ClientID = clientID;
        }
    }
}