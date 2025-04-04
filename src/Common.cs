using System.Reflection;
using CnSharp.Windows.Common;

namespace CnSharp.Windows.Forms.Editor
{
	public static class Common
	{
		public static string GetLocalText(string key)
		{
			return LocalizationHelper.GetLocalText(key, "CnSharp.Windows.Forms.Editor.Language", Assembly.GetExecutingAssembly());
		}
	}
}