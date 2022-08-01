using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MelonLoader;
using FC;
using FC.Utils;
using RootMotion.FinalIK;
using FusionClient;
using FusionClient.Modules;
using UnityEngine;
using System.Reflection;

namespace FusionClient.Modules
{
	[Obfuscation(Exclude = true, ApplyToMembers = true, StripAfterObfuscation = true)]
	public class IKTweeks 
    {
		public static bool LeftHandUp = false;
		public static bool RightHandUp = false;
		public static bool TwistHead = false;
		public static bool slingy = false;
		public static bool BrakeBones = false;

		public static void OnUpdate()
        {
			
			try
			{
				if (LeftHandUp && VRCPlayer.field_Internal_Static_VRCPlayer_0 != null)
				{
					FC.Utils.PlayerUtils.GetCurrentUser().gameObject.GetComponentInChildren<VRIK>().solver.leftArm.positionWeight = 1f;
				}
				if (RightHandUp && VRCPlayer.field_Internal_Static_VRCPlayer_0 != null)
				{
					FC.Utils.PlayerUtils.GetCurrentUser().gameObject.GetComponentInChildren<VRIK>().solver.rightArm.positionWeight = 1f;
				}

				if (TwistHead && VRCPlayer.field_Internal_Static_VRCPlayer_0 != null)
				{
					FC.Utils.PlayerUtils.GetCurrentUser().gameObject.GetComponentInChildren<VRIK>().solver.hasNeck = false;
				}
				else
				{
					FC.Utils.PlayerUtils.GetCurrentUser().gameObject.GetComponentInChildren<VRIK>().solver.hasNeck = true;
				}
				if (slingy && VRCPlayer.field_Internal_Static_VRCPlayer_0 != null)
				{
					FC.Utils.PlayerUtils.GetCurrentUser().gameObject.GetComponentInChildren<VRIK>().solver.hasChest = false;
				}
				else
				{
					FC.Utils.PlayerUtils.GetCurrentUser().gameObject.GetComponentInChildren<VRIK>().solver.hasChest = true;
				}
				if (BrakeBones && VRCPlayer.field_Internal_Static_VRCPlayer_0 != null)
				{
					VRIK componentInChildren2 = PlayerUtils.GetCurrentUser().gameObject.GetComponentInChildren<VRIK>();
					componentInChildren2.fixTransforms = false;
					componentInChildren2.animator.enabled = false;
				}
				else
				{
					VRIK componentInChildren2 = PlayerUtils.GetCurrentUser().gameObject.GetComponentInChildren<VRIK>();
					componentInChildren2.fixTransforms = true;
					componentInChildren2.animator.enabled = true;
				}
			}
			catch { }
		}
    }
}