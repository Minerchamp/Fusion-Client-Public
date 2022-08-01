using ExitGames.Client.Photon;
using FusionClient.Modules;
using MelonLoader;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnhollowerBaseLib;
using UnhollowerRuntimeLib;
using UnityEngine;
using VRC;
using VRC.Core;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;
using VRC.UI;
using System.Text.RegularExpressions;
using System.Globalization;
using UnhollowerRuntimeLib.XrefScans;
using VRC.UserCamera;
using FusionClient.Core;
using FusionClient.Utils.VRChat;

namespace FC.Utils
{
    [Obfuscation(Exclude = true, ApplyToMembers = true, StripAfterObfuscation = true)]
    internal static class MiscUtils
    {
        public static bool CheckUsed(MethodBase methodBase, string methodName)
        {
            try
            {
                return XrefScanner.UsedBy(methodBase).Where(instance => instance.TryResolve() != null && instance.TryResolve().Name.Contains(methodName)).Any();
            }
            catch { }
            return false;
        }

        internal static Texture2D CreateTextureFromBase64(string data)
        {
            Texture2D texture = new Texture2D(2, 2);
            ImageConversion.LoadImage(texture, Convert.FromBase64String(data));

            texture.hideFlags |= HideFlags.DontUnloadUnusedAsset;

            return texture;
        }

        internal static Sprite CreateSpriteFromBase64(string data)
        {
            Texture2D texture = CreateTextureFromBase64(data);

            Rect rect = new Rect(0.0f, 0.0f, texture.width, texture.height);

            Vector2 pivot = new Vector2(0.5f, 0.5f);
            Vector4 border = Vector4.zero;

            Sprite sprite = Sprite.CreateSprite_Injected(texture, ref rect, ref pivot, 100.0f, 0, SpriteMeshType.Tight, ref border, false);

            return sprite;
        }

        public static string RandomNumberString(int length)
        {
            string text = "";
            for (int i = 0; i < length; i++)
            {
                text += new System.Random().Next(0, 9);
            }
            return text;
        }

        public static Player getOwnerOfGameObject(GameObject gameObject)
        {
            foreach (Player player in PlayerUtils.GetPlayers())
            {
                bool flag = player.field_Private_VRCPlayerApi_0.IsOwner(gameObject);
                if (flag)
                {
                    return player;
                }
            }
            return null;
        }

        public static void TakeOwnershipIfNecessary(GameObject gameObject)
        {
            if (getOwnerOfGameObject(gameObject) != PlayerUtils.GetCurrentUser()._player)
            {
                Networking.SetOwner(PlayerUtils.GetCurrentUser().field_Private_VRCPlayerApi_0, gameObject);
            }
        }

        public static Il2CppArrayBase<UdonBehaviour> Behaviours;

        public static void SendUdonRPC(GameObject Object, string EventName, Player Target = null, bool Local = false)
        {
            if (Object != null)
            {
                var Behaviour = Object.GetComponent<UdonBehaviour>();
                if (Target != null)
                {
                    Networking.SetOwner(Target.field_Private_VRCPlayerApi_0, Object);
                    Behaviour.SendCustomNetworkEvent(NetworkEventTarget.Owner, EventName);
                }
                else
                {
                    if (!Local) Behaviour.SendCustomNetworkEvent(NetworkEventTarget.All, EventName);
                    else Behaviour.SendCustomEvent(EventName);
                }
            }
            else
            {
                foreach (var Behaviour in Behaviours)
                {
                    if (Behaviour._eventTable.ContainsKey(EventName)) Behaviour.SendCustomNetworkEvent(NetworkEventTarget.All, EventName);
                }
            }
        }

        public static GameObject PortalPosition(Vector3 position, Quaternion rotation)
        {
            GameObject gameObject = Networking.Instantiate(0, "Portals/PortalInternalDynamic", position, rotation);
            if (gameObject == null)
            {
                return null;
            }
            RPC.Destination destination = RPC.Destination.AllBufferOne;
            GameObject gameObject2 = gameObject;
            string text = "ConfigurePortal";
            Il2CppSystem.Object[] array = new Il2CppSystem.Object[3];
            array[0] = "FusionClient on top";
            array[1] = "FusionClient on top";
            int num = 2;
            Il2CppSystem.Int32 @int = default(Il2CppSystem.Int32);
            @int.m_value = -666;
            array[num] = @int.BoxIl2CppObject();
            Networking.RPC(destination, gameObject2, text, array);
            return gameObject;
        }


        internal static void EmojiRPC(int i)
        {
            try
            {
                Il2CppSystem.Int32 @int = default(Il2CppSystem.Int32);
                @int.m_value = i;
                Il2CppSystem.Object @object = @int.BoxIl2CppObject();
                Networking.RPC(0, PlayerUtils.GetCurrentUser().gameObject, "SpawnEmojiRPC", new Il2CppSystem.Object[]
                {
                    @object
                });
            }
            catch { }
        }

        internal static bool IsSDK2()
        {
            return GetSDK2Descriptor() != null;
        }

        internal static VRC_SceneDescriptor GetSDK2Descriptor()
        {
            return UnityEngine.Object.FindObjectOfType<VRC_SceneDescriptor>();
        }

        public static string RandomString(int length)
        {
            
            char[] array = " ̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺ͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩ".ToArray<char>();
            string text = "";
            System.Random random = new System.Random(new System.Random().Next(length));
            for (int i = 0; i < length; i++)
            {
                text += array[random.Next(array.Length)].ToString();
            }
            return text;
        }

        internal static void ForceQuit()
        {
            try
            {
                Process.GetCurrentProcess().Kill();
            }
            catch { }

            try
            {
                Environment.Exit(0);
            }
            catch { }

            try
            {
                Application.Quit();
            }
            catch { }
        }

        internal static void Restart()
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            if (PlayerUtils.SelfIsInVR())
            {
                psi.FileName = Environment.CurrentDirectory + "\\VRChat.exe";
            }
            else
            {
                psi.FileName = Environment.CurrentDirectory + "\\VRChat.exe";
                psi.Arguments = "--no-vr";
            }
            Process.Start(psi);
            ForceQuit();
        }

        internal static void LoginFailed()
        {
            
            Logs.Log("Authorization to Viper's Servers has failed. Please restart and check again!", ConsoleColor.Red);
            Console.ReadKey();
            ForceQuit();
        }

        internal static void ChangePedestals()
        {

            PopupUtils.InputPopup("Enter Avatar ID for pedestals", "avtr_XXXXXXXX", delegate (string s) 
            {
                if (WorldUtils.GetSDKType() == "SDK2")
                {
                    var cachedPedestals = UnityEngine.Object.FindObjectsOfType<VRC_AvatarPedestal>();
                    if (cachedPedestals.Count != 0)
                    {
                        foreach (var p in cachedPedestals)
                        {
                            p.blueprintId = s;
                        }
                        Logs.Debug($"<color=red>[EXPLOITS]</color> Changed <color=yellow>{cachedPedestals.Count}</color> pedestals!");
                    }
                    else
                    {
                        PopupUtils.HideCurrentPopUp();
                        PopupUtils.InformationAlert("No Pedestals!\nThere are currently no avatar pedestals to be found in this world!");
                    }
                }
                else
                {
                    var cachedPedestals = UnityEngine.Object.FindObjectsOfType<VRCAvatarPedestal>();
                    if (cachedPedestals.Count != 0)
                    {
                        foreach (var p in cachedPedestals)
                        {
                            p.blueprintId = s;
                        }
                        Logs.Debug($"<color=red>[EXPLOITS]</color> Changed <color=yellow>{cachedPedestals.Count}</color> pedestals!");
                    }
                    else
                    {
                        PopupUtils.HideCurrentPopUp();
                        PopupUtils.InformationAlert("No Pedestals!\nThere are currently no avatar pedestals to be found in this world!");
                    }
                }
            });
        }

        internal static System.Random random = new();

        internal static readonly List<string> EmojiType = new List<string>
        {
            ">:(", //0
            "Blushing", //1
            "Crying", //2
            "D:", //3
            "Wave", //4
            "Hang Loose", //5
            "Heart Eyes", //6
            "Pumpkin", //7
            "Kissy Face", //8
            "xD", //9
            "Skull", //10
            ":D", //11
            "Ghost", //12
            ":|", //13
            "B|", //14
            "Thinking", //15
            "Thumbs Down", //16
            "Thumbs Up", //17
            ":P", //18
            ":O", //19
            "Bats", //20
            "Cloud", //21
            "Fire", //22
            "Snowflakes", //23
            "Snowball", //24
            "Water Droplets", //25
            "Cobwebs", //26
            "Beer", //27
            "Candy", //28
            "Candy Cane", //29
            "Candy Corn", //30
            "Cheers", //31
            "Coconut Drink", //32
            "Gingerbread Man", //33
            "Ice Cream", //34
            "Pineapple", //35
            "Pizza", //36
            "Tomato", //37
            "Beach Ball", //38
            "Rock Gift", //39
            "Confetti", //40
            "Gift", //41
            "Christmas Gifts", //42
            "Lifebuoy", //43
            "Mistletoe", //44
            "Money", //45
            "Sunglasses", //46
            "Sunscreen", //47
            "BOO!", //48
            "</3", //49
            "!", //50
            "GO", //51
            "Heart", //52
            "Music Note", //53
            "?", //54
            "NO", //55
            "ZZZ", //56
        };
        internal static readonly List<string> EmoteType = new List<string>
        {
            "",
            "wave",
            "clap",
            "point",
            "cheer",
            "dance",
            "backflip",
            "die",
            "sadness",
        };

        internal static void DelayFunction(float del, Action action)
        {
            MelonCoroutines.Start(Delay(del, action));
        }

        private static IEnumerator Delay(float del, Action action)
        {
            yield return new WaitForSeconds(del);
            action.Invoke();
            yield break;
        }

        internal static void WaitForWorld(Action action)
        {
            MelonCoroutines.Start(WFW(action));
        }

        internal static IEnumerator WFW(Action action)
        {
            while (!WorldUtils.IsInWorld() && PlayerUtils.GetCurrentUser() == null) yield return null;
            action.Invoke();
            yield break;
        }

        public static string GetPath(GameObject obj)
        {
            string text = "/" + obj.name;
            while (obj.transform.parent != null)
            {
                obj = obj.transform.parent.gameObject;
                text = "/" + obj.name + text;
            }
            return text;
        }

        internal static void ChangeAvatar(this ApiAvatar instance)
        {
            try
            {
                new PageAvatar
                {
                    field_Public_SimpleAvatarPedestal_0 = new SimpleAvatarPedestal
                    {
                        field_Internal_ApiAvatar_0 = new ApiAvatar
                        {
                            id = instance.id
                        }
                    }
                }.ChangeToSelectedAvatar();
            }
            catch { Logs.Log("Error turning into avatar! Maybe it's non existing?", ConsoleColor.Red); }
        }

        internal static void ChangeAvatar(this Player instance)
        {
            try
            {
                new PageAvatar
                {
                    field_Public_SimpleAvatarPedestal_0 = new SimpleAvatarPedestal
                    {
                        field_Internal_ApiAvatar_0 = new ApiAvatar
                        {
                            id = instance.prop_ApiAvatar_0.id
                        }
                    }
                }.ChangeToSelectedAvatar();
            }
            catch { Logs.Log("Error turning into avatar! Maybe it's non existing?", ConsoleColor.Red); }
        }

        internal static void ChangeAvatar(this VRCPlayer instance)
        {
            try
            {
                new PageAvatar
                {
                    field_Public_SimpleAvatarPedestal_0 = new SimpleAvatarPedestal
                    {
                        field_Internal_ApiAvatar_0 = new ApiAvatar
                        {
                            id = instance.prop_ApiAvatar_0.id
                        }
                    }
                }.ChangeToSelectedAvatar();
            }
            catch { Logs.Log("Error turning into avatar! Maybe it's non existing?", ConsoleColor.Red); }
        }

        internal static void ChangeAvatar(string id)
        {
            try
            {
                new PageAvatar
                {
                    field_Public_SimpleAvatarPedestal_0 = new SimpleAvatarPedestal
                    {
                        field_Internal_ApiAvatar_0 = new ApiAvatar
                        {
                            id = id
                        }
                    }
                }.ChangeToSelectedAvatar();
            }
            catch { Logs.Log("Error turning into avatar! Maybe it's non existing?", ConsoleColor.Red); }
        }

        internal static Player GetPlayerByUserId(string userId)
        {
            foreach (var player in PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0)
                if (player.prop_APIUser_0 != null && player.prop_APIUser_0.id == userId)
                    return player;
            return null;
        }

        private static AssetBundle FusionBundle;
        public static Sprite NP1;
        public static Sprite NP2;
        public static Sprite Logo;
        public static Sprite MediaBack;
        public static Sprite MediaPause;
        public static Sprite MediaNext;
        public static Sprite MediaMute;

        internal static void LoadBundle()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("fusionclient")) //String is MainNamespace.assetbundlename
            using (var tempStream = new MemoryStream((int)stream.Length))
            {
                stream.CopyTo(tempStream);

                FusionBundle = AssetBundle.LoadFromMemory_Internal(tempStream.ToArray(), 0);
                FusionBundle.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            }

            NP1 = FusionBundle.LoadAsset_Internal("Assets/arial.ttf", Il2CppType.Of<Sprite>()).Cast<Sprite>(); //String is the location/name of the asset in the assetbundle
            NP1.hideFlags |= HideFlags.DontUnloadUnusedAsset;

            NP2 = FusionBundle.LoadAsset_Internal("Assets/OverRender.shader", Il2CppType.Of<Sprite>()).Cast<Sprite>(); //String is the location/name of the asset in the assetbundle
            NP2.hideFlags |= HideFlags.DontUnloadUnusedAsset;
        }

        public static void DropPortal(string RoomId)
        {
            string[] Location = RoomId.Split(':');
            DropPortal(Location[0], Location[1], 0,
                PlayerUtils.GetCurrentUser().transform.position + PlayerUtils.GetCurrentUser().transform.forward * 2f,
                PlayerUtils.GetCurrentUser().transform.rotation);
        }

        public static void DropPortal(string WorldID, string InstanceID, int players, Vector3 vector3, Quaternion quaternion)
        {
            GameObject gameObject = Networking.Instantiate(VRC_EventHandler.VrcBroadcastType.Always,
                "Portals/PortalInternalDynamic", vector3, quaternion);
            string world = WorldID;
            string instance = InstanceID;
            int count = players;
            Networking.RPC(RPC.Destination.AllBufferOne, gameObject, "ConfigurePortal", new Il2CppSystem.Object[]
            {
                (Il2CppSystem.String) world,
                (Il2CppSystem.String) instance,
                new Il2CppSystem.Int32
                {
                    m_value = count
                }.BoxIl2CppObject()
            });
            // MelonCoroutines.Start(MiscUtility.DestroyDelayed(1f, gameObject.GetComponent<PortalInternal>()));
        }

        public static void DropPortalInfPlayers(string RoomId)
        {
            string[] Location = RoomId.Split(':');
            DropPortalInfPlayers(Location[0], Location[1], 0,
                PlayerUtils.GetCurrentUser().transform.position + PlayerUtils.GetCurrentUser().transform.forward * 2f,
                PlayerUtils.GetCurrentUser().transform.rotation);
        }

        public static void DropPortalInfPlayers(string WorldID, string InstanceID, int players, Vector3 vector3, Quaternion quaternion)
        {
            GameObject gameObject = Networking.Instantiate(VRC_EventHandler.VrcBroadcastType.Always,
                "Portals/PortalInternalDynamic", vector3, quaternion);
            string world = WorldID;
            string instance = InstanceID;
            Networking.RPC(RPC.Destination.AllBufferOne, gameObject, "ConfigurePortal", new Il2CppSystem.Object[]
            {
                (Il2CppSystem.String) world,
                (Il2CppSystem.String) instance,
                new Il2CppSystem.Int32
                {
                    m_value = int.MinValue
            }.BoxIl2CppObject()
            });
            // MelonCoroutines.Start(MiscUtility.DestroyDelayed(1f, gameObject.GetComponent<PortalInternal>()));
        }

        public static byte[] GetByteArray(int sizeInKb)
        {
            System.Random random = new System.Random();
            byte[] array = new byte[sizeInKb * 1024];
            random.NextBytes(array);
            return array;
        }
        public static UnityEngine.Object ByteArrayToObjectUnity2(byte[] arrBytes)
        {
            Il2CppStructArray<byte> il2CppStructArray = new Il2CppStructArray<byte>((long)arrBytes.Length);
            arrBytes.CopyTo(il2CppStructArray, 0);
            Il2CppSystem.Object @object = new Il2CppSystem.Object(il2CppStructArray.Pointer);
            return new UnityEngine.Object(@object.Pointer);
        }
        public static byte[] ToByteArray(Il2CppSystem.Object obj)
        {
            if (obj == null) return null;
            var bf = new Il2CppSystem.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            var ms = new Il2CppSystem.IO.MemoryStream();
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }
        public static byte[] ToByteArray(object obj)
        {
            if (obj == null) return null;
            var bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            var ms = new System.IO.MemoryStream();
            bf.Serialize(ms, obj);
            return ms.ToArray();
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
        public static T IL2CPPFromByteArray<T>(byte[] data)
        {
            if (data == null) return default(T);
            var bf = new Il2CppSystem.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            var ms = new Il2CppSystem.IO.MemoryStream(data);
            object obj = bf.Deserialize(ms);
            return (T)obj;
        }
        public static T FromIL2CPPToManaged<T>(Il2CppSystem.Object obj)
        {
            return FromByteArray<T>(ToByteArray(obj));
        }
        public static T FromManagedToIL2CPP<T>(object obj)
        {
            return IL2CPPFromByteArray<T>(ToByteArray(obj));
        }

        public static object[] FromIL2CPPArrayToManagedArray(Il2CppSystem.Object[] obj)
        {
            object[] Parameters = new object[obj.Length];
            for (int i = 0; i < obj.Length; i++)
                if (obj[i].GetIl2CppType().Attributes == Il2CppSystem.Reflection.TypeAttributes.Serializable)
                    Parameters[i] = FromIL2CPPToManaged<object>(obj[i]);
                else
                    Parameters[i] = (object)obj[i];
            return Parameters;
        }

        public static Il2CppSystem.Object[] FromManagedArrayToIL2CPPArray(object[] obj)
        {
            Il2CppSystem.Object[] Parameters = new Il2CppSystem.Object[obj.Length];
            for (int i = 0; i < obj.Length; i++)
            {
                if (obj[i].GetType().Attributes == System.Reflection.TypeAttributes.Serializable)
                    Parameters[i] = FromManagedToIL2CPP<Il2CppSystem.Object>(obj[i]);
                else
                    Parameters[i] = (Il2CppSystem.Object)obj[i];
            }
            return Parameters;
        }

        public static UserCameraController UserCameraController
        {
            get
            {
                return UserCameraController.field_Internal_Static_UserCameraController_0;
            }
        }

        public static void SendRPC(VRC_EventHandler.VrcEventType EventType, string Name, GameObject ParamObject, int Int, float Float, string String, VRC_EventHandler.VrcBooleanOp Bool, VRC_EventHandler.VrcBroadcastType BroadcastType, Il2CppStructArray<byte> Byte = null)
        {
            if (ModulesFunctions.handler == null)
            {
                ModulesFunctions.handler = Resources.FindObjectsOfTypeAll<VRC_EventHandler>()[0];
            }

            VRC_EventHandler.VrcEvent a = new VRC_EventHandler.VrcEvent
            {
                EventType = EventType,
                Name = Name,
                ParameterObject = ParamObject,
                ParameterInt = Int,
                ParameterFloat = Float,
                ParameterString = String,
                ParameterBoolOp = Bool,
                ParameterBytes = Byte,
            };
            ModulesFunctions.handler.TriggerEvent(a, BroadcastType, ParamObject, 0f);
        }

        public static void OpRaiseEvent(byte code, object customObject, Photon.Realtime.RaiseEventOptions RaiseEventOptions, SendOptions sendOptions)
        {
            Il2CppSystem.Object Object = FromManagedToIL2CPP<Il2CppSystem.Object>(customObject);
            OpRaiseEvent(code, Object, RaiseEventOptions, sendOptions);
        }

        public static bool OpRaiseEvent(byte code, Il2CppSystem.Object customObject, Photon.Realtime.RaiseEventOptions RaiseEventOptions, SendOptions sendOptions)
        {
            //return PhotonPeerPublicPo1PaTyUnique.Method_Public_Virtual_New_Boolean_Byte_Object_RaiseEventOptions_SendOptions_0
            return PhotonNetwork.field_Public_Static_LoadBalancingClient_0.Method_Public_Virtual_New_Boolean_Byte_Object_RaiseEventOptions_SendOptions_0(code, customObject, RaiseEventOptions, sendOptions);
            return PhotonNetwork.Method_Public_Static_Boolean_Byte_Object_RaiseEventOptions_SendOptions_0(code, customObject, RaiseEventOptions, sendOptions);
        }
    }
}
