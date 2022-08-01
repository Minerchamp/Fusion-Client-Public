using System;
using System.Reflection;

[Obfuscation(Exclude = true, ApplyToMembers = true, StripAfterObfuscation = true)]
internal class VMProtect : Attribute
{
	public VMProtect(string A_1)
	{
	}
}
