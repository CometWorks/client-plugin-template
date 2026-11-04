using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using VRageMath;

namespace ClientPlugin.Settings.Tools;

public static class Tools
{
    private static readonly Regex UpperCaseWordRegex = new(
        "[A-Z][a-z]*",
        RegexOptions.Compiled
    );

    public static string GetLabelOrDefault(string name, string label = null)
    {
        Debug.Assert(!string.IsNullOrEmpty(name) && name.Trim().Length != 0);

        if (label != null)
            return label;

        var words = UpperCaseWordRegex.Matches(name).Cast<Match>().Select(m => m.Value).ToArray();
        Debug.Assert(words.Length != 0);

        for (var i = 1; i < words.Length; i++)
        {
            words[i] = words[i].ToLower();
        }

        return string.Join(" ", words);
    }

    private static readonly Regex RxHexColorRgbRegex = new(
        "([0-9a-f]{2})([0-9a-f]{2})([0-9a-f]{2})",
        RegexOptions.IgnoreCase
    );
    private static readonly Regex RxHexColorRgbaRegex = new(
        "([0-9a-f]{2})([0-9a-f]{2})([0-9a-f]{2})([0-9a-f]{2})",
        RegexOptions.IgnoreCase
    );

    extension(Color color)
    {
        public string ToHexStringRgb()
        {
            return $"{color.R:X2}{color.G:X2}{color.B:X2}";
        }

        public string ToHexStringRgba()
        {
            return $"{color.R:X2}{color.G:X2}{color.B:X2}{color.A:X2}";
        }
    }

    extension(string hex)
    {
        public bool TryParseColorFromHexRgb(out Color color)
        {
            var match = RxHexColorRgbRegex.Match(hex);
            if (!match.Success)
            {
                color = Color.Black;
                return false;
            }

            color = new Color(
                Convert.ToInt16(match.Groups[1].Value, 16),
                Convert.ToInt16(match.Groups[2].Value, 16),
                Convert.ToInt16(match.Groups[3].Value, 16),
                255
            );
            return true;
        }

        public bool TryParseColorFromHexRgba(out Color color)
        {
            var match = RxHexColorRgbaRegex.Match(hex);
            if (!match.Success)
            {
                color = Color.Transparent;
                return false;
            }

            color = new Color(
                Convert.ToInt16(match.Groups[1].Value, 16),
                Convert.ToInt16(match.Groups[2].Value, 16),
                Convert.ToInt16(match.Groups[3].Value, 16),
                Convert.ToInt16(match.Groups[4].Value, 16)
            );
            return true;
        }
    }
}
