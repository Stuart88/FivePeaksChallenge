using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace FivePeaks.Server.Models
{
    public class GeoJson
    {
        public string type { get; set; }
        public GeoJsonProperties properties { get; set; }
        public GeoJsonGeometry geometry { get; set; }
    }

    public class GeoJsonProperties
    {
        public string name { get; set; }
    }

    public class GeoJsonGeometry
    {
        public string type { get; set; }
        public List<List<double[]>> coordinates { get; set; }
    }

}
