using FusionClient.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FC.Utils
{
    internal static class PhotonUtils
    {
        public static string GetUserID(this Photon.Realtime.Player player)
        {
            
            if (player.GetRawHashtable().ContainsKey("user"))
                if (player.GetHashtable()["user"] is Dictionary<string, object> dict)
                    return (string)dict["id"];
            return "No ID";
        }
        public static string GetDisplayName(this Photon.Realtime.Player player)
        {
            if (player.GetRawHashtable().ContainsKey("user"))
                if (player.GetHashtable()["user"] is Dictionary<string, object> dict)
                    return (string)dict["displayName"];
            return "No DisplayName";
        }

        public static int GetPhotonID(this Photon.Realtime.Player player)
            => player.field_Private_Int32_0;

        public static VRC.Player GetPlayer(this Photon.Realtime.Player player)
            => player.field_Public_Player_0;

        public static System.Collections.Hashtable GetHashtable(this Photon.Realtime.Player player)
            => FromIL2CPPToManaged<System.Collections.Hashtable>(player.GetRawHashtable());

        public static Il2CppSystem.Collections.Hashtable GetRawHashtable(this Photon.Realtime.Player player)
            => player.prop_Hashtable_0;

        public static List<Photon.Realtime.Player> GetAllPhotonPlayers(this Photon.Realtime.LoadBalancingClient Instance)
        {
            List<Photon.Realtime.Player> result = new List<Photon.Realtime.Player>();
            foreach (var x in Instance.prop_Room_0.prop_Dictionary_2_Int32_Player_0)
                result.Add(x.Value);
            return result;
        }

        public  static Photon.Realtime.Player GetPhotonPlayer(this Photon.Realtime.LoadBalancingClient Instance, int photonID)
        {
            foreach (var x in Instance.GetAllPhotonPlayers())
                if (x.GetPhotonID() == photonID)
                    return x;
            return null;
        }

        internal static Il2CppSystem.Collections.Generic.Dictionary<int, Photon.Realtime.Player> _PhotonPlayers =>
        Photon.Pun.PhotonNetwork.prop_Room_0.prop_Dictionary_2_Int32_Player_0;
        internal static Photon.Realtime.Player TryGetPlayer(int photonId)
        {
            try
            {
                foreach (var photonPlayer in _PhotonPlayers)
                {
                    if (photonPlayer.value.field_Private_Int32_0 == photonId)
                    {
                        return photonPlayer.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.Error("Try Get Player", ex);
            }
            return null;
        }

        public static Photon.Realtime.Player GetPhotonPlayer(this Photon.Realtime.LoadBalancingClient Instance, string userID)
        {
            foreach (var x in Instance.GetAllPhotonPlayers())
                if (x.GetUserID() == userID)
                    return x;
            return null;
        }

        public static T FromIL2CPPToManaged<T>(Il2CppSystem.Object obj)
        {
            //if (obj.GetIl2CppType().Attributes == Il2CppSystem.Reflection.TypeAttributes.Serializable)
            //    return obj.Cast<T>();
            return FromByteArray<T>(ToByteArray(obj));
        }

        public static T FromByteArray<T>(byte[] data)
        {
            if (data == null) return default(T);
            var bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            using (var ms = new System.IO.MemoryStream(data))
            {
                object obj = bf.Deserialize(ms);
                return (T)obj;
            }
        }

        public static byte[] ToByteArray(Il2CppSystem.Object obj)
        {
            if (obj == null) return null;
            var bf = new Il2CppSystem.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            var ms = new Il2CppSystem.IO.MemoryStream();
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }
    }
}
