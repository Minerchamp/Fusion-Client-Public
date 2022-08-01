using UnityEngine;

namespace FC.Utils
{
    internal static class ColorManager
    {
        internal static Color HexToColor(string hexCode)
        {
            ColorUtility.DoTryParseHtmlColor(hexCode, out var color);
            return color;
        }

        internal static Color32 HexToColor32(string hexCode)
        {
            ColorUtility.DoTryParseHtmlColor(hexCode, out var color);
            return color;
        }

        internal static string ColorToHex(Color color)
        {
            return ColorUtility.ToHtmlStringRGB(color);
        }

        internal static string Color32ToHex(Color32 color)
        {
            return ColorUtility.ToHtmlStringRGB(color);
        }
    }
}