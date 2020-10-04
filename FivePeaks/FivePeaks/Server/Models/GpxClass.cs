using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FivePeaks.Server.Models
{
    public class GpxClass
    {
        public string Name { get; set; }
        public string Time { get; set; }
        
        /// <summary>
        /// Track points
        /// </summary>
        public List<TrkPt> TrkPts { get; set; }

        public GeoJson ToGeoJson(string name)
        {
            GeoJson geo = new GeoJson
            {
                type = "Feature",
                properties = new GeoJsonProperties {name = name},
                geometry = new GeoJsonGeometry
                {
                    type = "MultiLineString"
                }
            };

            List<double[]> trkpts = new List<double[]>();

            //first ensure trkpts all have value that can be parsed
            foreach (var i in this.TrkPts)
            {
                if (!double.TryParse(i.Elevation, out _)) i.Elevation = "0";
                if (!double.TryParse(i.Lat, out _)) i.Lat = "0";
                if (!double.TryParse(i.Long, out _)) i.Long = "0";

                 trkpts.Add(new[] {double.Parse(i.Long), double.Parse(i.Lat), double.Parse(i.Elevation)});
            }

            geo.geometry.coordinates = new List<List<double[]>>{trkpts};
            
            return geo;
        }
    }

    public class TrkPt
    {
        public string Lat{ get; set; }
        public string Long{ get; set; }
        public string Elevation { get; set; }
        public string Time { get; set; }
    }
}
