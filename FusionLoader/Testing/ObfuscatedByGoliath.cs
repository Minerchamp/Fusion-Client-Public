using System;
using System.Reflection;

[Obfuscation(Exclude = true, ApplyToMembers = true, StripAfterObfuscation = true)]
internal class ObfuscatedByGoliath : Attribute
{
	public ObfuscatedByGoliath(string A_1)
	{
	}
}
