using System;
using System.Reflection;

namespace SecureTeam.Attributes
{
	[Obfuscation(Exclude = true, ApplyToMembers = true, StripAfterObfuscation = true)]
	internal class ObfuscatedByCliSecureAttribute : Attribute
	{
		public ObfuscatedByCliSecureAttribute(string A_1)
		{
		}
	}
}
