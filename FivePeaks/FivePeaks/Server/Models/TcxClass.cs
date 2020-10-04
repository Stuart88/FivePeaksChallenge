using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FivePeaks.Server.Models
{
    public class TcxClass
    {
        public string Name { get; set; }
        public List<TrackPoint> Trackpoints { get; set; }

        public GeoJson ToGeoJson(string name)
        {
            GeoJson geo = new GeoJson
            {
                type = "Feature",
                properties = new GeoJsonProperties { name = name },
                geometry = new GeoJsonGeometry
                {
                    type = "MultiLineString"
                }
            };

            List<double[]> trkpts = new List<double[]>();

            //first ensure trkpts all have value that can be parsed
            foreach (var i in this.Trackpoints)
            {
                if (!double.TryParse(i.Altitude, out _)) i.Altitude = "0";
                if (!double.TryParse(i.Position.Lat, out _)) i.Position.Lat = "0";
                if (!double.TryParse(i.Position.Long, out _)) i.Position.Long = "0";

                trkpts.Add(new[] { double.Parse(i.Position.Long), double.Parse(i.Position.Lat), double.Parse(i.Altitude) });
            }

            geo.geometry.coordinates = new List<List<double[]>> { trkpts };

            return geo;
        }
    }

    public class TrackPoint
    {
        public string Time { get; set; }
        public TcxPosition Position { get; set; }
        public string Altitude { get; set; }
    }

    public class TcxPosition
    {
        public string Lat { get; set; }
        public string Long { get; set; }
    }
}
