using System;
using System.Reflection;

namespace Xenocode.Client.Attributes.AssemblyAttributes
{
	[Obfuscation(Exclude = true, ApplyToMembers = true, StripAfterObfuscation = true)]
	internal class ProcessedByXenocode : Attribute
	{
		public ProcessedByXenocode(string A_1)
		{
		}
	}
}
