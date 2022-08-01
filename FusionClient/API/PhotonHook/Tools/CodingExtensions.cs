namespace FusionClient.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    public static class CodingExtensions
    {
        public static bool IsEmpty<T>(this List<T> list) => list == null || list.Count == 0;

        public static bool IsNotEmpty<T>(this List<T> list) => list.Count != 0;

        public static bool IsNull<T>(this T obj) where T : class => obj == null;

        public static bool IsNull<T>(this T? obj) where T : struct => !obj.HasValue;

        public static bool IsNotNull<T>(this T obj) where T : class => obj != null;

        public static bool IsNotNull<T>(this T? obj) where T : struct => obj.HasValue;

        public static bool IsNotNullOrEmptyOrWhiteSpace(this string obj) => !string.IsNullOrEmpty(obj) && !string.IsNullOrWhiteSpace(obj);

        internal static string ReplaceWholeWord(this string original, string wordToFind, string replacement, RegexOptions regexOptions = RegexOptions.IgnoreCase)
        {
            return Regex.Replace(original, wordToFind, replacement, regexOptions);
        }

        internal static bool isMatch(this string value, string wordToFind, RegexOptions regexOptions = RegexOptions.IgnoreCase)
        {
            return Regex.IsMatch(value, wordToFind, regexOptions);
        }

        internal static string RemoveWhitespace(this string str)
        {
            return string.Join("", str.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
        }

        internal static bool IsPositive(this int number)
        {
            return number > 0;
        }
        internal static bool isDigitsOnly(this string s)
        {
            int len = s.Length;
            for (int i = 0; i < len; ++i)
            {
                char c = s[i];
                if (c < '0' || c > '9')
                    return false;
            }
            return true;
        }

        internal static bool IsNegative(this int number)
        {
            return number < 0;
        }
       internal static bool CheckRange(this float num, float min, float max)
        {
            return num > min && num < max;
        }

        internal static string Truncate(this string value, int max_length)
        {
            if (!string.IsNullOrEmpty(value) && value.Length > max_length)
                return value.Substring(0, max_length) + "…";
            return value;
        }

        internal static string ConvertToString<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        {
            return "{" + string.Join(",", dictionary.Select(kv => kv.Key + "=" + kv.Value).ToArray()) + "}";
        }

    }
}