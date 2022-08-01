using FusionClient.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using VRC;
using VRC.Core;
using VRC.SDKBase;
using VRC.Udon;

namespace FC.Utils.Managers
{
    internal static class WorldManager
    {
        internal static IEnumerable<Player> GetPlayers()
        {
            return VRC.PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0.ToArray();
        }

        internal static Il2CppSystem.Collections.Generic.List<Player> GetPlayers2()
        {
            return VRC.PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0;
        }

        internal static int GetPlayerCount()
        {
            return VRC.PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0.Count;
        }

        internal static string GetFullID()
        {
            return RoomManager.field_Internal_Static_ApiWorldInstance_0.id;
        }

        internal static InstanceAccessType GetInstanceType()
        {
            return RoomManager.field_Internal_Static_ApiWorldInstance_0.type;
        }

        internal static int GetFriendCount()
        {
            int results = 0;
            foreach (var p in GetPlayers2())
            {
                if (p.IsFriend())
                    results++;
            }
            return results;
        }

        internal static bool IsSDK2()
        {
            if (GetSDK2Descriptor() != null) return true;
            else return false;
        }

        internal static bool IsSDK3()
        {
            if (GetSDK3Descriptor() != null) return true;
            else return false;
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

        internal static VRCPlayer GetInstanceCreator()
        {
            foreach (Player p in GetPlayers())
            {
                if (p._vrcplayer.IsInstanceCreator())
                {
                    return p._vrcplayer;
                }
            }
            return null;
        }

        internal static bool IsDefaultScene(string name)
        {
            var lower = name.ToLower();
            string[] scenes = { "application2", "ui", "empty", "dontdestroyonload", "hideanddontsave", "samplescene" };
            if (scenes.Contains(lower))
                return true;
            else return false;
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

        /*internal static void GetInstanceInfo(string fullID)
        {
            VRC.Core.API.SendGetRequest($"worlds/{fullID.Split(':')[0]}/{fullID.Split(':')[1]}", new ApiContainer() 
            {
                OnSuccess = new Action<ApiContainer>((container) =>
                {
                    ApiModelContainer<ApiWorldInstance> instance = new ApiModelContainer<ApiWorldInstance>();
                    instance.setFromContainer(container);
                    var World = container.Model.Cast<ApiWorldInstance>();
                    Console.WriteLine("Succesful Call!");
                }),
                OnError = new Action<ApiContainer>((container) =>
                {
                    Console.WriteLine(container.Text);
                })
            });
        }*/

        internal static void GetInstances(string worldid, Action<ApiWorld> OnSuccess, Action OnError)
        {
            VRC.Core.API.SendGetRequest($"worlds/{worldid.Split(':')[0]}/{worldid.Split(':')[1]}", new ApiContainer()
            {
                OnSuccess = new Action<ApiContainer>((container) =>
                {
                    ApiModelContainer<ApiWorld> instance = new ApiModelContainer<ApiWorld>();
                    instance.setFromContainer(container);
                    var World = container.Model.Cast<ApiWorld>();
                    // Console.WriteLine(World.f);
                    OnSuccess?.Invoke(World);
                }),
                OnError = new Action<ApiContainer>((container) =>
                {
                    Console.WriteLine(container.Text);
                    OnError?.Invoke();
                })
            }, null, false, 3600);
        }

        internal static void JoinWorld(string fullID)
        {
            if (!fullID.ToLower().StartsWith("wrld_") || !fullID.ToLower().Contains(':'))
            {
                //Logs.Debug("<color=red>INVALID JOIN ID!</color>");
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
