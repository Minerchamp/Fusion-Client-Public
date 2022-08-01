using FC.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FusionClient.Modules.BotStuff
{
    class Optimizer : MonoBehaviour
    {
        float time = 0;
        float UpdateRate = 10;
        int RamCleanRate = 20;
        int RamCleanTime = 0;

        void Start()
        {
            OptimizeGame();
            OptimizeIngame();
        }

        void Update()
        {
            try
            {
                time += Time.deltaTime;
                if (time > UpdateRate)
                {
                    if (WorldUtils.IsInWorld())
                    {
                        OptimizeIngame();
                    }
                    OptimizeGame();
                    time = 0;
                }
            }
            catch
            {
            }
            ExecuteExploits();
        }

        void ExecuteExploits()
        {
            //todo
        }
        void OptimizeIngame()
        {
            foreach (TextMesh t in Resources.FindObjectsOfTypeAll<TextMesh>())
            {
                if (t != null && !t.gameObject.isStaticBatchable)
                {
                    t.gameObject.SetActive(false);
                    if (!t.gameObject.scene.name.Contains("Application"))
                    {
                        try
                        { UnityEngine.Object.Destroy(t); }
                        catch { }
                    }
                }
            }
            foreach (Renderer Render in Resources.FindObjectsOfTypeAll<Renderer>())
            {
                if (Render != null && !Render.gameObject.isStaticBatchable)
                {
                    Render.enabled = false;
                    if (!Render.gameObject.scene.name.Contains("Application"))
                    {
                        try
                        { UnityEngine.Object.Destroy(Render); }
                        catch { }
                    }
                }
            }
            foreach (SkinnedMeshRenderer Render2 in Resources.FindObjectsOfTypeAll<SkinnedMeshRenderer>())
            {
                if (Render2 != null && !Render2.gameObject.isStaticBatchable)
                {
                    Render2.enabled = false;
                    if (!Render2.gameObject.scene.name.Contains("Application"))
                    {
                        try
                        { UnityEngine.Object.Destroy(Render2); }
                        catch { }
                    }
                }
            }
            foreach (var Audio in Resources.FindObjectsOfTypeAll<AudioSource>())
            {
                if (Audio != null && !Audio.gameObject.isStaticBatchable)
                {
                    Audio.enabled = false;
                    try
                    { UnityEngine.Object.Destroy(Audio); }
                    catch { }

                }
            }
            foreach (var video in Resources.FindObjectsOfTypeAll<SyncVideoPlayer>())
            {
                if (video != null)
                {
                    video.enabled = false;
                    if (!video.gameObject.scene.name.Contains("Application"))
                    {
                        try
                        { UnityEngine.Object.Destroy(video); }
                        catch { }
                    }
                }
            }
            //if (RoomManagerExtension.IsInWorld())
            //{
            //    if (Camera.main != null)
            //    {
            //        Camera.main.farClipPlane = 2f;
            //        Camera.main.fieldOfView = 25;
            //        Camera.main.nearClipPlane = 1;
            //    }
            //    RamCleanTime++;
            //    if (RamCleanTime > RamCleanRate && RoomManagerExtension.IsInWorld())
            //    {
            //        MiscUtility.ClearRam();
            //        System.GC.Collect();
            //        System.GC.WaitForPendingFinalizers();
            //        RamCleanTime = 0;
            //    }
            //}
        }
        void OptimizeGame()
        {
            Application.targetFrameRate = 25;
            Screen.SetResolution(25, 25, false);
        }

        public Optimizer(IntPtr ptr) : base(ptr)
        {
        }
    }
}
