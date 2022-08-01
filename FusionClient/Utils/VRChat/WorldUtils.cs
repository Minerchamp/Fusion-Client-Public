using FusionClient.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using UnhollowerRuntimeLib;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.VR;
using UnityEngine.XR;
using VRC;
using VRC.Core;
using VRC.SDKBase;
using VRC.Udon;

namespace FC.Utils
{
    internal static class WorldUtils
    {

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

        public static List<UdonBehaviour> Get_UdonBehaviours()
        {
            var UdonBehaviourObjects = new List<UdonBehaviour>();
            var list = Resources.FindObjectsOfTypeAll<UdonBehaviour>();
            if (list.Count() != 0)
            {
                foreach (var item in list)
                {
                    if (item._eventTable.Keys.Count != 0)
                    {
                        if (!UdonBehaviourObjects.Contains(item))
                        {
                            UdonBehaviourObjects.Add(item);
                        }
                    }
                }
                return UdonBehaviourObjects;
            }
            return UdonBehaviourObjects;
        }

        public static Vector3 GetWorldCameraPosition()
        {
            
            VRCVrCamera camera = VRCVrCamera.field_Private_Static_VRCVrCamera_0;
            var type = camera.GetIl2CppType();
            if (type == Il2CppType.Of<VRCVrCameraSteam>())
            {
                VRCVrCameraSteam steam = camera.Cast<VRCVrCameraSteam>();
                Transform transform1 = steam.field_Private_Transform_0;
                Transform transform2 = steam.field_Private_Transform_1;
                if (transform1.name == "Camera (eye)")
                {
                    return transform1.position;
                }
                if (transform2.name == "Camera (eye)")
                {
                    return transform2.position;
                }
            }
            if (type == Il2CppType.Of<VRCVrCameraUnity>())
            {
                VRCVrCameraUnity unity = camera.Cast<VRCVrCameraUnity>();
                return unity.field_Public_Camera_0.transform.position;
            }
            if (type == Il2CppType.Of<VRCVrCameraWave>())
            {
                VRCVrCameraWave wave = camera.Cast<VRCVrCameraWave>();
                return wave.transform.position;
            }
            return camera.transform.parent.TransformPoint(GetLocalCameraPosition());
        }

        public static Vector3 GetLocalCameraPosition()
        {
            VRCVrCamera camera = VRCVrCamera.field_Private_Static_VRCVrCamera_0;
            var type = camera.GetIl2CppType();
            if (type == Il2CppType.Of<VRCVrCamera>())
            {
                return camera.transform.localPosition;
            }
            if (type == Il2CppType.Of<VRCVrCameraSteam>())
            {
                VRCVrCameraSteam steam = camera.Cast<VRCVrCameraSteam>();
                Transform transform1 = steam.field_Private_Transform_0;
                Transform transform2 = steam.field_Private_Transform_1;
                if (transform1.name == "Camera (eye)")
                {
                    return camera.transform.parent.InverseTransformPoint(transform1.position);
                }
                if (transform2.name == "Camera (eye)")
                {
                    return camera.transform.parent.InverseTransformPoint(transform2.position);
                }
                else
                {
                    return Vector3.zero;
                }
            }
            if (type == Il2CppType.Of<VRCVrCameraUnity>())
            {
                if (PlayerUtils.SelfIsInVR())
                {
                    return camera.transform.localPosition + UnityEngine.XR.InputTracking.GetLocalPosition(XRNode.CenterEye);
                }
                VRCVrCameraUnity unity = camera.Cast<VRCVrCameraUnity>();
                return camera.transform.parent.InverseTransformPoint(unity.field_Public_Camera_0.transform.position);
            }
            if (type == Il2CppType.Of<VRCVrCameraWave>())
            {
                VRCVrCameraWave wave = camera.Cast<VRCVrCameraWave>();
                return wave.field_Public_Transform_0.InverseTransformPoint(camera.transform.position);
            }
            return Vector3.zero;
        }

        internal static IEnumerable<Player> GetPlayers()
        {
            return PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0.ToArray();
        }

        internal static List<Player> GetPlayersToList()
        {
            return PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0.ToArray().ToList();
        }

        internal static List<Player> GetPlayers(this PlayerManager Instance)
        {
            return Instance.field_Private_List_1_Player_0.ToArray().ToList();
        }

        internal static int GetPlayerCount()
        {
            return PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0.Count;
        }

        internal static VRCPlayer GetInstanceMaster()
        {
            foreach (Player p in GetPlayers())
            {
                if (p._vrcplayer.IsInstanceMaster())
                {
                    return p._vrcplayer;
                }
            }
            return null;
        }

        internal static ApiWorld GetCurrentWorld()
        {
            return RoomManager.field_Internal_Static_ApiWorld_0;
        }

        internal static ApiWorldInstance GetCurrentInstance()
        {
            return RoomManager.field_Internal_Static_ApiWorldInstance_0;
        }

        internal static bool IsInWorld()
        {
            if (RoomManager.field_Internal_Static_ApiWorld_0 != null && RoomManager.field_Internal_Static_ApiWorldInstance_0 != null) return true;
            else return false;
        }

        internal static string GetWorldID()
        {
            return RoomManager.field_Internal_Static_ApiWorld_0.id;
        }

        internal static string GetFullID()
        {
            return $"{RoomManager.field_Internal_Static_ApiWorldInstance_0.id}";
        }

        internal static string GetSDKType()
        {
            if (GetSDK2Descriptor() != null)
                return "SDK2";
            else if (GetSDK3Descriptor() != null)
                return "SDK3";
            else
                return "not found";
        }

        internal static void JoinWorld(string fullID)
        {
            if (!fullID.ToLower().StartsWith("wrld_") || !fullID.ToLower().Contains('#'))
            {
                Logs.Debug("<color=red>INVALID JOIN ID!</color>");
                Logs.Log("INVALID JOIN ID!", ConsoleColor.Red);
                return;
            }
            else
            {
                Networking.GoToRoom(fullID);
            }
        }

        internal static void JoinWorld(string worldID, string instanceID)
        {
            if (!worldID.ToLower().StartsWith("wrld_"))
            {
                //Logs.Debug("<color=red>INVALID WORLD ID!</color>");
                Logs.Log("INVALID WORLD ID!", ConsoleColor.Red);
                return;
            }
            else
            {
                new PortalInternal().Method_Private_Void_String_String_PDM_0(worldID, instanceID);
            }
        }

        internal static bool IsDefaultScene(string name)
        {
            var lower = name.ToLower();
            string[] scenes = { "application2", "ui", "empty", "dontdestroyonload", "hideanddontsave", "samplescene" };
            if (scenes.Contains(lower))
                return true;
            else return false;
        }

        internal static VRCSDK2.VRC_SceneDescriptor GetSDK2Descriptor()
        {
            return UnityEngine.Object.FindObjectOfType<VRCSDK2.VRC_SceneDescriptor>();
        }

        internal static VRC.SDK3.Components.VRCSceneDescriptor GetSDK3Descriptor()
        {
            return UnityEngine.Object.FindObjectOfType<VRC.SDK3.Components.VRCSceneDescriptor>();
        }

        internal static VRC_Pickup[] GetPickups()
        {
            return Resources.FindObjectsOfTypeAll<VRC_Pickup>();
        }

        internal static VRC_Pickup[] GetActivePickups()
        {
            return UnityEngine.Object.FindObjectsOfType<VRC_Pickup>();
        }

        internal static VRC_Interactable[] GetInteractables()
        {
            return Resources.FindObjectsOfTypeAll<VRC_Interactable>();
        }

        internal static VRC_Interactable[] GetActiveInteractables()
        {
            return UnityEngine.Object.FindObjectsOfType<VRC_Interactable>();
        }

        internal static PostProcessVolume[] GetBloom()
        {
            return Resources.FindObjectsOfTypeAll<PostProcessVolume>();
        }

        internal static PostProcessVolume[] GetActiveBloom()
        {
            return UnityEngine.Object.FindObjectsOfType<PostProcessVolume>();
        }

        internal static UdonBehaviour[] GetUdonScripts()
        {
            return Resources.FindObjectsOfTypeAll<UdonBehaviour>();
        }

        internal static UdonBehaviour[] GetActiveUdonScripts()
        {
            return UnityEngine.Object.FindObjectsOfType<UdonBehaviour>();
        }

        internal static VRC_MirrorReflection[] GetMirrors()
        {
            return Resources.FindObjectsOfTypeAll<VRC_MirrorReflection>();
        }

        internal static VRC_MirrorReflection[] GetActiveMirrors()
        {
            return UnityEngine.Object.FindObjectsOfType<VRC_MirrorReflection>();
        }
    }
}
