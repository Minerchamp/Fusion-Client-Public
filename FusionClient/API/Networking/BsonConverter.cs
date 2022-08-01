namespace Fusion;

using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

public static class BsonConverter
{
    public static byte[] ToBson<T>(T data)
    {
        using MemoryStream ms = new();
        using BsonDataWriter datawriter = new(ms);
        JsonSerializer serializer = new();
        serializer.Serialize(datawriter, data);
        return ms.ToArray();
    }

    public static T FromBson<T>(byte[] data)
    {
        using MemoryStream ms = new(data);
        using BsonDataReader reader = new(ms);
        JsonSerializer serializer = new();
        return serializer.Deserialize<T>(reader);
    }
}
