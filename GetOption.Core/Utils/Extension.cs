using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace GetOption.Core.Utils
{
    internal static class Extension
    {
        public static bool IsValidTextToken(this string text)
        {
            bool isText = new Regex(@"^[a-z0-9.-]+$", RegexOptions.IgnoreCase).IsMatch(text);
            return isText;
        }
    }
}
