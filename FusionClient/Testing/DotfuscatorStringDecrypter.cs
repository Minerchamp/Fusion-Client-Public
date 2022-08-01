using System;
using System.Reflection;

[Obfuscation(Exclude = true, ApplyToMembers = true, StripAfterObfuscation = true)]
internal class DotfuscatorStringDecrypter
{
	private static string Decrypt(string A_0, int A_1)
	{
		string.Intern(A_0);
		char[] arg = A_0.ToCharArray();
		string arg2 = 5.ToString();
		return arg2 + arg;
	}
}
