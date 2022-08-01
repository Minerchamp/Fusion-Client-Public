using System;
using System.Reflection;

namespace SecureTeam.Attributes
{
	[Obfuscation(Exclude = true, ApplyToMembers = true, StripAfterObfuscation = true)]
	internal class ObfuscatedByAgileDotNetAttribute : Attribute
	{
		public ObfuscatedByAgileDotNetAttribute(string A_1)
		{
		}
	}
}
