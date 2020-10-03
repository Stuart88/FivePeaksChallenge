using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FivePeaks.Client.Helpers
{
    public static class ServerHelpers
    {
        public static string ImageSrcString(string filePath, bool compress = false)
        {
            return compress
                ? $"Site/GetImageCompressed/{System.Web.HttpUtility.UrlEncode(filePath)}"
                : $"Site/GetImage/{System.Web.HttpUtility.UrlEncode(filePath)}";
        }

    }
}
