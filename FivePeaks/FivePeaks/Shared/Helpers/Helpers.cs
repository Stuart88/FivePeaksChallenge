using System;
using System.Collections.Generic;
using System.Text;

namespace FivePeaks.Shared.Helpers
{
    public static class Blog
    {
        public static List<string> BlogTags = new List<string>
        {
            "Hike",
            "Road Trip",
            "Yorkshire",
            "Scotland",
            "Highlands",
            "England",
            "Lake District",
            "Info",
            "Yorkshire Dales"
        };
    }

    public static class Functions
    {
        public static string MakeUrlSafe(string s)
        {
            return System.Net.WebUtility.UrlEncode(s.Replace(" ", "-")
                .Replace("_", "-")
                .Replace(".", "-")
                .Replace("!", "-")
                .Replace("*", "-")
                .Replace("(", "-")
                .Replace(")", "-")
            );
        }
    }
}
