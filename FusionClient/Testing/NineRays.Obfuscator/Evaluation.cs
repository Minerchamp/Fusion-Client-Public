using System;
using System.Reflection;

namespace NineRays.Obfuscator
{
	[Obfuscation(Exclude = true, ApplyToMembers = true, StripAfterObfuscation = true)]
	internal class Evaluation : Attribute
	{
		public Evaluation(string A_1)
		{
		}
	}
}
