using System;
using System.Reflection;

namespace NineRays.Obfuscator
{
	[Obfuscation(Exclude = true, ApplyToMembers = true, StripAfterObfuscation = true)]
	internal class SoftwareWatermarkAttribute : Attribute
	{
		public SoftwareWatermarkAttribute(string A_1)
		{
		}
	}
}
