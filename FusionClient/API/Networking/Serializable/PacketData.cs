namespace Fusion.Networking.Serializable
{
    using System;
    using System.Reflection;

    [Serializable, Obfuscation]
    public class PacketData
    {
        public byte NetworkEventID;

        public string TextData = string.Empty;

        public PacketData(byte networkEventID, string textData = "")
        {
            NetworkEventID = networkEventID;
            TextData = textData;
        }
    }
}