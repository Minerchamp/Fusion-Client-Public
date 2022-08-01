using Il2CppSystem.Text;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnhollowerBaseLib.Attributes;
using UnityEngine;
using Valve.VR;
using static System.Net.Mime.MediaTypeNames;

namespace FusionClient.Modules
{
    [Obfuscation(Exclude = true, ApplyToMembers = true, StripAfterObfuscation = true)]
    class GetTimeLeft : MonoBehaviour
    {
        public GetTimeLeft(IntPtr id) : base(id) { }

        public void Update()
        {
            //GetTimeLeft.delay -= Time.deltaTime;
            //if (GetTimeLeft.delay < 0f)
            //{
            //	try
            //	{
            //		ETrackedPropertyError etrackedPropertyError = 0;
            //		int num = 0;
            //		for (uint num2 = 0u; num2 < 16u; num2 += 1u)
            //		{
            //			StringBuilder stringBuilder = new StringBuilder(64);
            //			OpenVR.System.GetStringTrackedDeviceProperty(num2, (ETrackedDeviceProperty)1003, stringBuilder, 64u, ref etrackedPropertyError);
            //			ETrackedPropertyError etrackedPropertyError2 = 0;
            //			float floatTrackedDeviceProperty = OpenVR.System.GetFloatTrackedDeviceProperty(num2, (ETrackedDeviceProperty)1012, ref etrackedPropertyError2);
            //			string text = stringBuilder + "|" + (floatTrackedDeviceProperty * 100f).ToString();
            //			if (text != "|0")
            //			{
            //				GetTime.Text.text = text;
            //			}
            //			else
            //			{
            //				GetTime.Text.text = "";
            //			}
            //		}
            //	}
            //	catch (Exception)
            //	{
            //	}
            //	GetTimeLeft.delay = 5f;
            //}
            GetTime.Text.text = $"<b>[Expires] \n 2024:1:1";
        }

		// Token: 0x04000249 RID: 585
		public static float delay = 5f;

		// Token: 0x0400024A RID: 586
		public static int BatteryTextCount = 0;

		[HideFromIl2Cpp]
        internal static DateTime UnixTimeToDateTime(long unixtime)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Local);
            dtDateTime = dtDateTime.AddSeconds(unixtime).ToLocalTime();
            return dtDateTime;
        }
    }
}
