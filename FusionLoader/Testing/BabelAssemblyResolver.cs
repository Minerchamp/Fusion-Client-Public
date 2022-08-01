using System;
using System.Collections;
using System.IO;
using System.Reflection;
[Obfuscation(Exclude = true, ApplyToMembers = true, StripAfterObfuscation = true)]
// Token: 0x020000D7 RID: 215
internal class BabelAssemblyResolver
{
	// Token: 0x0600062E RID: 1582 RVA: 0x0003EF68 File Offset: 0x0003D168
	private static void Register()
	{
		try
		{
			AppDomain.CurrentDomain.AssemblyResolve += BabelAssemblyResolver.OnAssemblyResolve;
		}
		catch (Exception)
		{
		}
	}

	// Token: 0x0600062F RID: 1583 RVA: 0x0003EFA0 File Offset: 0x0003D1A0
	private static Assembly OnAssemblyResolve(object A_0, ResolveEventArgs A_1)
	{
		Assembly result;
		try
		{
			result = null;
		}
		catch (Exception)
		{
			result = null;
		}
		return result;
	}

	// Token: 0x06000630 RID: 1584 RVA: 0x000022D0 File Offset: 0x000004D0
	private static void Decrypt(Stream A_0)
	{
	}

	// Token: 0x0400048A RID: 1162
	private object _o;

	// Token: 0x0400048B RID: 1163
	private int _i;

	// Token: 0x0400048C RID: 1164
	private Hashtable _h;
}
