using System;
using System.Collections.Generic;
using System.Reflection;
[Obfuscation(Exclude = true, ApplyToMembers = true, StripAfterObfuscation = true)]
internal class GoliathStrongNameChecker
{
	public static void AntiTamper(Type A_0)
	{
		Exception ex = new Exception();
		throw ex;
	}

	public byte[] RequiredMethod(Assembly A_1)
	{
		return null;
	}

	public string RequiredMethod(Stack<int> A_1)
	{
		return null;
	}

	public int RequiredMethod(int A_1, byte[] A_2)
	{
		return 0;
	}
}
