using System;
using System.Reflection;

[Obfuscation(Exclude = true, ApplyToMembers = true, StripAfterObfuscation = true)]
internal class DotfuscatorAttribute : Attribute
{
	public DotfuscatorAttribute(string A_1)
	{
	}
}
