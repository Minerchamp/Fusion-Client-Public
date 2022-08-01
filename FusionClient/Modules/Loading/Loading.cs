using FC.Utils;
using FusionClient.Core;
using FusionClient.Utils.Manager;
using MelonLoader;
using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using UnityEngine.UI;
using VRC.Core;

namespace FusionClient.Modules
{
    internal class Loading : FusionModule
    {
        public override void UI()
        {
            if (Config.Main.CustomLoading)
            {
                MelonCoroutines.Start(ReplaceLoading());
                MelonCoroutines.Start(Particles());
            }
        }

        private static IEnumerator ReplaceLoading()
        {
            if (Config.Main.CustomLoading)
            {
                GameObject gameObject = GameObject.Find("LoadingBackground_TealGradient_Music/LoadingSound");
                GameObject gameObject2 = GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/LoadingSound");
                if (File.Exists("Fusion Client\\Misc\\LoadingSong.ogg"))
                {
                    UnityWebRequest www = UnityWebRequest.Get("file://" + Path.Combine(Environment.CurrentDirectory, "Fusion Client\\Misc\\LoadingSong.ogg"));
                    www.SendWebRequest();
                    while (!www.isDone)
                    {
                        yield return null;
                    }
                    if (!www.isHttpError)
                    {
                        AudioClip audioClip2 = WebRequestWWW.InternalCreateAudioClipUsingDH(www.downloadHandler, www.url, false, false, 0);
                        gameObject.GetComponent<AudioSource>().clip = audioClip2;
                        gameObject2.GetComponent<AudioSource>().clip = audioClip2;
                        audioClip2.name = "Sound";
                        VRCUiManager.field_Private_Static_VRCUiManager_0.transform.Find("LoadingBackground_TealGradient_Music/LoadingSound").GetComponent<AudioSource>().clip = audioClip2;
                        VRCUiManager.field_Private_Static_VRCUiManager_0.transform.Find("MenuContent/Popups/LoadingPopup/LoadingSound").GetComponent<AudioSource>().clip = audioClip2;
                        VRCUiManager.field_Private_Static_VRCUiManager_0.transform.Find("LoadingBackground_TealGradient_Music/LoadingSound").GetComponent<AudioSource>().maxVolume = 3000f;
                        VRCUiManager.field_Private_Static_VRCUiManager_0.transform.Find("MenuContent/Popups/LoadingPopup/LoadingSound").GetComponent<AudioSource>().maxVolume = 3000f;
                        VRCUiManager.field_Private_Static_VRCUiManager_0.transform.Find("LoadingBackground_TealGradient_Music/LoadingSound").GetComponent<AudioSource>().volume = 2999f;
                        VRCUiManager.field_Private_Static_VRCUiManager_0.transform.Find("MenuContent/Popups/LoadingPopup/LoadingSound").GetComponent<AudioSource>().volume = 2999f;
                        VRCUiManager.field_Private_Static_VRCUiManager_0.transform.Find("LoadingBackground_TealGradient_Music/LoadingSound").GetComponent<AudioSource>().Play();
                        VRCUiManager.field_Private_Static_VRCUiManager_0.transform.Find("MenuContent/Popups/LoadingPopup/LoadingSound").GetComponent<AudioSource>().Play();
                    }
                    www = null;
                }
                else
                {
                    if (!File.Exists("Fusion Client\\Misc\\LoadingSong.ogg"))
                    {
                        WebClient webClient = new WebClient();
                        webClient.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                        webClient.DownloadFile("https://vrcmods.xyz/assets/others/LoadingMusic.ogg", $"Fusion Client\\Misc\\LoadingSong.ogg");
                    }
                    if (gameObject != null)
                    {
                        gameObject.GetComponent<AudioSource>().Stop();
                    }
                    if (gameObject2 != null)
                    {
                        gameObject2.GetComponent<AudioSource>().Stop();
                    }
                    UnityWebRequest unityWebRequest = UnityWebRequest.Get("https://vrcmods.xyz/assets/others/LoadingMusic.ogg");
                    unityWebRequest.SendWebRequest();
                    while (!unityWebRequest.isDone)
                    {
                        yield return null;
                    }
                    AudioClip audioClip = WebRequestWWW.InternalCreateAudioClipUsingDH(unityWebRequest.downloadHandler, unityWebRequest.url, false, false, 0);
                    while (!unityWebRequest.isDone)
                    {
                    }
                    if (audioClip != null)
                    {
                        if (gameObject != null)
                        {
                            gameObject.GetComponent<AudioSource>().clip = audioClip;
                            gameObject.GetComponent<AudioSource>().Play();
                        }
                        if (gameObject2 != null)
                        {
                            gameObject2.GetComponent<AudioSource>().clip = audioClip;
                            gameObject2.GetComponent<AudioSource>().Play();
                        }
                    }
                }
                yield return new WaitForSecondsRealtime(1f);
            }
        }

        public static Color TextColor()
        {
            return HexToColor("#008cff");
        }

        public static Color HexToColor(string hexColor)
        {
            if (hexColor.IndexOf('#') != -1)
            {
                hexColor = hexColor.Replace("#", "");
            }
            float r = int.Parse(hexColor.Substring(0, 2), NumberStyles.AllowHexSpecifier) / 255f;
            float g = int.Parse(hexColor.Substring(2, 2), NumberStyles.AllowHexSpecifier) / 255f;
            float b = int.Parse(hexColor.Substring(4, 2), NumberStyles.AllowHexSpecifier) / 255f;
            return new Color(r, g, b);
        }

        public static IEnumerator Particles()
        {
            if (Config.Main.CustomLoading)
            {
                    var Border = GameObject.Find("/UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingInfoPanel/InfoPanel_Template_ANIM/SCREEN/mainFrame").GetComponent<MeshRenderer>();
                    var PointLight = GameObject.Find("/UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingBackground_TealGradient/_Lighting (1)/Point light").GetComponent<Light>();
                    var ReflectionProbe1 = GameObject.Find("/UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingBackground_TealGradient/_Lighting (1)/Reflection Probe").GetComponent<ReflectionProbe>();
                    while (Border == null) yield return null;
                    while (PointLight == null) yield return null;
                    while (ReflectionProbe1 == null) yield return null;
                    ReflectionProbe1.mode = ReflectionProbeMode.Realtime;
                    ReflectionProbe1.backgroundColor = new Color(0.4006691f, 0, 1, 0);
                    Material BorderMaterial = new Material(Shader.Find("Standard"));
                    Border.material = BorderMaterial;
                    Border.material.color = Color.black;
                    Border.material.SetFloat("_Metallic", 1);
                    Border.material.SetFloat("_SmoothnessTextureChar", 1);
                    PointLight.color = Color.white;

                    var Snow = GameObject.Find("/UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingBackground_TealGradient/_FX_ParticleBubbles/FX_snow").GetComponent<ParticleSystem>();
                    var Snow2 = GameObject.Find("/UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingBackground_TealGradient/_FX_ParticleBubbles/FX_snow").GetComponent<ParticleSystemRenderer>();
                    while (Snow == null) yield return null;
                    while (Snow2 == null) yield return null;

                    Snow.gameObject.SetActive(false);
                    //Snow.gameObject.transform.position -= new Vector3(0, -2.5f, 0);
                    Snow.gameObject.transform.position -= new Vector3(0, -1, 0);
                    Material TrailMaterial = new Material(Shader.Find("UI/Default"));
                    TrailMaterial.color = Color.white;
                    Snow2.trailMaterial = TrailMaterial;
                    Snow2.material.color = Color.white;

                    //Trail
                    Snow.trails.enabled = true;
                    Snow.trails.mode = ParticleSystemTrailMode.PerParticle;
                    Snow.trails.ratio = 1;
                    Snow.trails.lifetime = 0.1f;
                    Snow.trails.minVertexDistance = 0;
                    Snow.trails.worldSpace = false;
                    Snow.trails.dieWithParticles = true;
                    Snow.trails.textureMode = ParticleSystemTrailTextureMode.Stretch;
                    Snow.trails.sizeAffectsWidth = true;
                    Snow.trails.sizeAffectsLifetime = false;
                    Snow.trails.inheritParticleColor = false;
                    Snow.trails.colorOverLifetime = Color.white;
                    Snow.trails.widthOverTrail = 0.1f;
                    Snow.trails.colorOverTrail = new Color(0, 0.1f, 0.1962264f, 0.5f);

                    //MainParticle
                    //Snow.shape.scale = new Vector3(1, 1, 1.82f);
                    Snow.shape.scale = new Vector3(2, 2, 2.82f);
                    Snow.main.startColor.mode = ParticleSystemGradientMode.Color;
                    Snow.colorOverLifetime.enabled = false;
                    Snow.main.prewarm = false;
                    Snow.playOnAwake = true;
                    Snow.startColor = new Color(0, 0.3f, 0.8f, 1);
                    Snow.noise.frequency = 1;
                    Snow.noise.strength = 0.5f;
                    Snow.maxParticles = 350;
                    Snow.gameObject.SetActive(true);

                    var CloseParticles = GameObject.Find("/UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingBackground_TealGradient/_FX_ParticleBubbles/FX_CloseParticles");
                    while (CloseParticles == null) yield return null;
                    CloseParticles.GetComponent<ParticleSystem>().startColor = TextColor();

                    var Floor = GameObject.Find("/UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingBackground_TealGradient/_FX_ParticleBubbles/FX_floor");
                    while (Floor == null) yield return null;
                    Floor.GetComponent<ParticleSystem>().startColor = TextColor();

                    var Snow3 = GameObject.Find("/UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingBackground_TealGradient/_FX_ParticleBubbles/FX_snow").GetComponent<ParticleSystem>();
                    var Snow4 = GameObject.Find("/UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingBackground_TealGradient/_FX_ParticleBubbles/FX_snow").GetComponent<ParticleSystemRenderer>();
                    while (Snow3 == null) yield return null;
                    while (Snow4 == null) yield return null;

                    Snow3.gameObject.SetActive(false);
                    Snow3.gameObject.transform.position -= new Vector3(0, 5, 0);
                    TrailMaterial.color = Color.white;
                    Snow4.trailMaterial = TrailMaterial;
                    Snow4.material.color = Color.white;
                    //Snow2.material.mainTexture = new Texture();

                    //Trail
                    Snow3.trails.enabled = true;
                    Snow3.trails.mode = ParticleSystemTrailMode.PerParticle;
                    Snow3.trails.ratio = 1;
                    Snow3.trails.lifetime = 0.04f;
                    Snow3.trails.minVertexDistance = 0;
                    Snow3.trails.worldSpace = false;
                    Snow3.trails.dieWithParticles = true;
                    Snow3.trails.textureMode = ParticleSystemTrailTextureMode.Stretch;
                    Snow3.trails.sizeAffectsWidth = true;
                    Snow3.trails.sizeAffectsLifetime = false;
                    Snow3.trails.inheritParticleColor = false;
                    Snow3.trails.colorOverLifetime = Color.white;
                    Snow3.trails.widthOverTrail = 0.1f;
                    Snow3.trails.colorOverTrail = new Color(0, 0.1f, 0.1962264f, 0.5f);

                    //MainParticle
                    Snow3.shape.scale = new Vector3(1, 1, 1.82f);
                    Snow3.main.startColor.mode = ParticleSystemGradientMode.Color;
                    Snow3.colorOverLifetime.enabled = false;
                    Snow3.main.prewarm = false;
                    Snow3.playOnAwake = true;
                    Snow3.startColor = new Color(0, 0.3f, 0.8f, 1);
                    Snow3.noise.frequency = 1;
                    Snow3.noise.strength = 0.5f;
                    Snow3.maxParticles = 350;
                    Snow3.gameObject.SetActive(true);

                    var CloseParticles1 = GameObject.Find("/UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingBackground_TealGradient/_FX_ParticleBubbles/FX_CloseParticles");
                    while (CloseParticles1 == null) yield return null;
                    CloseParticles1.GetComponent<ParticleSystem>().startColor = TextColor();

                    var Floor1 = GameObject.Find("/UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingBackground_TealGradient/_FX_ParticleBubbles/FX_floor");
                    while (Floor1 == null) yield return null;
                    Floor1.GetComponent<ParticleSystem>().startColor = TextColor();

                    var LoadingBar = GameObject.Find("/UserInterface/MenuContent/Popups/LoadingPopup/ProgressPanel/Parent_Loading_Progress/Loading Elements/LOADING_BAR_BG").GetComponent<Image>();
                    while (LoadingBar == null) yield return null;

                    var LoadingBar1 = GameObject.Find("/UserInterface/MenuContent/Popups/LoadingPopup/ProgressPanel/Parent_Loading_Progress/Loading Elements/LOADING_BAR").GetComponent<Image>();
                    while (LoadingBar1 == null) yield return null;

                    var LoadingBarPanel = GameObject.Find("/UserInterface/MenuContent/Popups/LoadingPopup/ProgressPanel/Parent_Loading_Progress/Panel_Backdrop").GetComponent<Image>();
                    while (LoadingBarPanel == null) yield return null;

                    var LoadingBarPanelRight = GameObject.Find("/UserInterface/MenuContent/Popups/LoadingPopup/ProgressPanel/Parent_Loading_Progress/Decoration_Right").GetComponent<Image>();
                    while (LoadingBarPanelRight == null) yield return null;

                    var LoadingBarPanelLeft = GameObject.Find("/UserInterface/MenuContent/Popups/LoadingPopup/ProgressPanel/Parent_Loading_Progress/Decoration_Left").GetComponent<Image>();
                    while (LoadingBarPanelLeft == null) yield return null;

                    var BackGround = GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingBackground_TealGradient/SkyCube_Baked").active = false;

                    GameObject.Find("/UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingInfoPanel").active = false;
            }
        }
    }
}