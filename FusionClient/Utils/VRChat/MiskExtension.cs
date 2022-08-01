using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnhollowerRuntimeLib.XrefScans;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR;
using VRC.SDKBase;

namespace FusionClient.Other
{
    public static class MiskExtension
    {
        public static GameObject GetUniqueGameObject(string name)
        {
            var Gameobjects = SceneManager.GetActiveScene().GetRootGameObjects().ToArray();
            //var Gameobjects = Resources.FindObjectsOfTypeAll<GameObject>().ToArray();
            for (int i = 0; i < Gameobjects.Length; i++)
                if (Networking.GetUniqueName(Gameobjects[i]) == name)
                    return Gameobjects[i];
            return null;
        }

        public static void SetToolTipBasedOnToggle(this UiTooltip tooltip)
        {
            UiToggleButton componentInChildren = tooltip.gameObject.GetComponentInChildren<UiToggleButton>();

            if (componentInChildren != null && !string.IsNullOrEmpty(tooltip.GetAlternateText()))
            {
                string displayText = (!componentInChildren.field_Public_Boolean_0) ? tooltip.GetAlternateText() : tooltip.GetText();
                if (TooltipManager.field_Private_Static_Text_0 != null) //Only return type field of text
                    TooltipManager.Method_Public_Static_Void_String_0(displayText); //Last function to take string parameter
                if (tooltip.GetToolTip() != null)
                    tooltip.GetToolTip().text = displayText;
            }
        }

        internal static Text GetToolTip(this UiTooltip Instance)
        {
            return Instance.field_Public_Text_0;
        }

        internal static string GetText(this UiTooltip Instance)
        {
            return Instance.field_Public_String_0;
        }

        internal static string GetAlternateText(this UiTooltip Instance)
        {
            return Instance.field_Public_String_1;
        }

        internal static bool GetToggledOn(this UiTooltip Instance)
        {
            return Instance.field_Private_Boolean_0;
        }

        public static void EnterPortal(this PortalInternal Instance, string WorldID, string InstanceID)
        {
            Instance.Method_Private_Void_String_String_PDM_0(WorldID, InstanceID);
        }

        public static bool IsInVR()
        {
            return XRDevice.isPresent;
        }

        [Obsolete("This Doenst always works -Day <3")]
        public static bool IsCurrentWorldUdon()
        {
            return RoomManager.field_Internal_Static_ApiWorld_0.tags.Contains("Udon");
        }

        public static bool XRefScanForGlobal(this MethodBase methodBase, string searchTerm, bool ignoreCase = true)
        {
            if (!string.IsNullOrEmpty(searchTerm))
                return XrefScanner.XrefScan(methodBase).Any(
                    xref => xref.Type == XrefType.Global && xref.ReadAsObject()?.ToString().IndexOf(
                                searchTerm,
                                ignoreCase
                                    ? StringComparison.OrdinalIgnoreCase
                                    : StringComparison.Ordinal) >= 0);
            MelonLogger.Error($"XRefScanForGlobal \"{methodBase}\" has an empty searchTerm. Returning false");
            return false;
        }

        public static bool XRefScanForMethod(this MethodBase methodBase, string methodName = null, string parentType = null, bool ignoreCase = true)
        {
            if (!string.IsNullOrEmpty(methodName)
                || !string.IsNullOrEmpty(parentType))
                return XrefScanner.XrefScan(methodBase).Any(
                    xref =>
                    {
                        if (xref.Type != XrefType.Method) return false;

                        var found = false;
                        MethodBase resolved = xref.TryResolve();
                        if (resolved == null) return false;

                        if (!string.IsNullOrEmpty(methodName))
                            found = !string.IsNullOrEmpty(resolved.Name) && resolved.Name.IndexOf(
                                    methodName,
                                    ignoreCase
                                        ? StringComparison.OrdinalIgnoreCase
                                        : StringComparison.Ordinal) >= 0;

                        if (!string.IsNullOrEmpty(parentType))
                            found = !string.IsNullOrEmpty(resolved.ReflectedType?.Name) && resolved.ReflectedType.Name.IndexOf(
                                    parentType,
                                    ignoreCase
                                        ? StringComparison
                                            .OrdinalIgnoreCase
                                        : StringComparison.Ordinal)
                                >= 0;

                        return found;
                    });
            MelonLogger.Error($"XRefScanForMethod \"{methodBase}\" has all null/empty parameters. Returning false");
            return false;
        }

        public static int XRefScanMethodCount(this MethodBase methodBase, string methodName = null, string parentType = null, bool ignoreCase = true)
        {
            if (!string.IsNullOrEmpty(methodName)
                || !string.IsNullOrEmpty(parentType))
                return XrefScanner.XrefScan(methodBase).Count(
                    xref =>
                    {
                        if (xref.Type != XrefType.Method) return false;

                        var found = false;
                        MethodBase resolved = xref.TryResolve();
                        if (resolved == null) return false;

                        if (!string.IsNullOrEmpty(methodName))
                            found = !string.IsNullOrEmpty(resolved.Name) && resolved.Name.IndexOf(
                                    methodName,
                                    ignoreCase
                                        ? StringComparison.OrdinalIgnoreCase
                                        : StringComparison.Ordinal) >= 0;

                        if (!string.IsNullOrEmpty(parentType))
                            found = !string.IsNullOrEmpty(resolved.ReflectedType?.Name) && resolved.ReflectedType.Name.IndexOf(
                                    parentType,
                                    ignoreCase
                                        ? StringComparison
                                            .OrdinalIgnoreCase
                                        : StringComparison.Ordinal)
                                >= 0;

                        return found;
                    });
            MelonLogger.Error($"XRefScanMethodCount \"{methodBase}\" has all null/empty parameters. Returning -1");
            return -1;
        }

        public static bool checkXref(this MethodBase m, string match)
        {
            try
            {
                return XrefScanner.XrefScan(m).Any(
                    instance => instance.Type == XrefType.Global && instance.ReadAsObject() != null && instance.ReadAsObject().ToString()
                                   .Equals(match, StringComparison.OrdinalIgnoreCase));
            }
            catch { }

            return false;
        }

        //  I Know is dumb, but it works.
        public static void DestroyMeLocal(this UnityEngine.Object obj)
        {
            if (obj != null)
            {
                var name = obj.name;
                if (obj != null)
                {
                    UnityEngine.Object.DestroyImmediate(obj);
                }
                if (obj != null)
                {
                    UnityEngine.Object.Destroy(obj);
                }
                if (obj != null)
                {
                    UnityEngine.Object.DestroyObject(obj);
                }
                if (obj != null)
                {
                    MelonLogger.Log("Failed To Destroy Object : " + obj.name);
                    MelonLogger.Log("Try To Destroy His GameObject in case you are trying to destroy the transform.");
                }
                else
                {
                    MelonLogger.Log("Destroyed Client-side Object : " + name);
                }
            }
        }

        public static List<List<T>> SplitIntoChunks<T>(List<T> list, int chunkSize)
        {
            if (chunkSize <= 0)
            {
                throw new ArgumentException("chunkSize must be greater than 0.");
            }

            List<List<T>> retVal = new List<List<T>>();
            int index = 0;
            while (index < list.Count)
            {
                int count = list.Count - index > chunkSize ? chunkSize : list.Count - index;
                retVal.Add(list.GetRange(index, count));

                index += chunkSize;
            }

            return retVal;
        }

        public static T Cast<T>(this object o)
        {
            return (T)o;
        }

        public static List<List<T>> SplitList<T>(this List<T> Instance, int Count)
        {
            List<List<T>> ReturnList = new List<List<T>>();
            int Splits = Instance.Count / Count;
            for (int i = 0; i < Splits; i++)
                ReturnList[Count * i].AddRange(Instance.GetRange(i * Count, Count));
            Console.WriteLine($"List Count: {Instance.Count} | Count: {Count} | Splits: {Count}");
            return ReturnList;
        }

        internal static bool IsRunningNotorious()
        {
            bool _return = false;
            _return = Assembly.GetExecutingAssembly().GetType("Notorious") != null;
            return _return;
        }
    }
}