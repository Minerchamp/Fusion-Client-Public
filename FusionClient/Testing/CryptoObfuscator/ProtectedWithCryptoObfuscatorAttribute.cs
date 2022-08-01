using System;
using System.Reflection;

namespace CryptoObfuscator
{
	[Obfuscation(Exclude = true, ApplyToMembers = true, StripAfterObfuscation = true)]
	internal class ProtectedWithCryptoObfuscatorAttribute : Attribute
	{
		public ProtectedWithCryptoObfuscatorAttribute(string A_1)
		{
		}
	}
}
