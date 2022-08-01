namespace Fusion.Networking
{
    using System;
    using System.Reflection;

    [Serializable, Obfuscation]
    public class AuthData
    {
        public byte ClientType;

        public string Key = string.Empty;

        public AuthData(byte clientType, string key = "")
        {
            ClientType = clientType;
            Key = key;
        }
    }
}