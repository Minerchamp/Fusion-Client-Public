using System;
using System.Reflection;

namespace SmartAssembly.Attributes
{
	[Obfuscation(Exclude = true, ApplyToMembers = true, StripAfterObfuscation = true)]
	internal class PoweredByAttribute : Attribute
	{
		public PoweredByAttribute(string A_1)
		{
		}
	}
}
