using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using FivePeaks.Server.Database;
using FivePeaks.Server.Models;
using FivePeaks.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FivePeaks.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LeaderboardController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly FivePeaksDbContext _context;

        public LeaderboardController(IWebHostEnvironment environment)
        {
            _environment = environment;
            _context = new FivePeaksDbContext();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> UpdateItem([FromBody] LeaderboardItem leaderboardItem)
        {
            try
            {
                if (!string.IsNullOrEmpty(leaderboardItem.RouteData) && leaderboardItem.RouteData.Length > 0)
                {
                    CheckValidXml(leaderboardItem.RouteData, leaderboardItem.RouteDataType);
                }

                LeaderboardItem entry = await _context.Leaderboard.AsNoTracking().FirstOrDefaultAsync(x => x.Id == leaderboardItem.Id);

                if (entry == null)
                {
                    leaderboardItem.Status = LeaderboardEntryState.Pending;
                    await _context.Leaderboard.AddAsync(leaderboardItem);
                }
                else
                {
                    //these two items stay empty on client side, so transfer over to updated entry.
                    leaderboardItem.Id = entry.Id;
                    leaderboardItem.RouteData = entry.RouteData;

                    _context.Leaderboard.Update(leaderboardItem);
                }

                await _context.SaveChangesAsync();

                return Ok(new BasicHttpResponse<object> { Ok = true, Data = leaderboardItem});
            }
            catch (Exception e)
            {
                return Ok(new BasicHttpResponse<object> { Ok = false, Message = Shared.Helpers.Functions.ExceptionMessage(e) });
            }
        }

        private void CheckValidXml(string xml, string fileType)
        {
            try
            {
                Regex tagsWithData = new Regex("<\\w+>[^<]+</\\w+>");

                //Light checking
                if (string.IsNullOrEmpty(xml) || tagsWithData.IsMatch(xml) == false)
                {
                    throw new Exception("Badly formed document!");
                }

                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(xml);

            }
            catch (Exception e)
            {
                throw new Exception($"Route Data Error: {Shared.Helpers.Functions.ExceptionMessage(e)}");
            }
        }



        [AllowAnonymous]
        [HttpGet("[action]")]
        public IActionResult GetAllUsernames()
        {
            try
            {
                var users = _context.Users.OrderBy(u => u.Username).Select(s => new UserAccount
                {
                    Id = s.Id,
                    Username = s.Username
                }).ToList();

                return Ok(new BasicHttpResponse<List<UserAccount>> { Ok = true, Data = users });
            }
            catch (Exception e)
            {
                return Ok(new BasicHttpResponse<object> { Ok = false, Message = e.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("[action]/{userId}")]
        public IActionResult GetSubmissions(int userId = 0)
        {
            try
            {

                var items = userId switch
                {
                    // Admin
                    -1 => _context.Leaderboard
                        .Select(s => Helpers.Helpers.LeaderboardItemWithoutRouteData(s))
                        .ToList(),
                    // Public
                    0 => _context.Leaderboard
                        .Where(i => i.Status == LeaderboardEntryState.Accepted)
                        .Select(s => Helpers.Helpers.LeaderboardItemWithoutRouteData(s))
                        .ToList(),
                    // Specific userId
                    _ => _context.Leaderboard
                        .Where(i => i.PostedByUserId == userId)
                        .Select(s => Helpers.Helpers.LeaderboardItemWithoutRouteData(s))
                        .ToList()
                };

                return Ok(new BasicHttpResponse<List<LeaderboardItem>> { Ok = true, Data = items });
            }
            catch (Exception e)
            {
                return Ok(new BasicHttpResponse<object> { Ok = false, Message = e.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("[action]/{entryId}")]
        public async Task<IActionResult> GetRouteData(int entryId)
        {
            try
            {
                var entry = await _context.Leaderboard.FindAsync(entryId);

                if (!string.IsNullOrEmpty(entry.RouteData))
                {
                   
                    MemoryStream ms = new MemoryStream();
                    TextWriter tw = new StreamWriter(ms);

                    await tw.WriteAsync(entry.RouteData);

                    await tw.FlushAsync();

                    byte[] bytes = ms.ToArray();
                    ms.Close();

                    return File(bytes, $"application/{entry.RouteDataType.Replace(".", "")}+xml", $"{entry.TrekDate:dd-MMM-yyyy}_{entry.EntryName}{entry.RouteDataType}");
                }

                throw new Exception("Nothing found!");

            }
            catch (Exception e)
            {
                return Ok($"Error: {Shared.Helpers.Functions.ExceptionMessage(e)}");
            }
        }

        [AllowAnonymous]
        [HttpGet("[action]/{entryId}")]
        public async Task<IActionResult> GetRouteDataAsGeoJSON(int entryId)
        {
            try
            {
                var entry = await _context.Leaderboard.FindAsync(entryId);
                
                if (entry == null)
                    throw new Exception("No data found!");

                if (!string.IsNullOrEmpty(entry.RouteData))
                {
                    XDocument xml = XDocument.Parse(entry.RouteData);

                    if (xml.Document == null)
                    {
                        throw new Exception("Entry has no route data!");
                    }

                    if (entry.RouteDataType == ".gpx")
                    {
                        GpxClass gpx = new GpxClass();

                        gpx.Name = xml.Document.Descendants().FirstOrDefault(x => x.Name.LocalName == "name")?.Value ?? "";
                        gpx.Time = xml.Document.Descendants().FirstOrDefault(x => x.Name.LocalName == "time")?.Value ?? "";

                        gpx.TrkPts = xml.Document.Descendants().Where(x => x.Name.LocalName == "trkpt").Select(s => new TrkPt
                            {
                                Elevation = s.Descendants().FirstOrDefault(x => x.Name.LocalName == "ele")?.Value ?? "",
                                Lat = s.Attribute("lat")?.Value ?? "",
                                Long = s.Attribute("lon")?.Value ?? "",
                                Time = s.Descendants().FirstOrDefault(x => x.Name.LocalName == "time")?.Value ?? "",
                            })
                            .ToList();

                        return Ok(new BasicHttpResponse<GeoJson>() { Ok = true, Data = gpx.ToGeoJson(entry.EntryName) });
                    }
                    else if (entry.RouteDataType == ".tcx")
                    {
                        TcxClass tcx = new TcxClass();

                        tcx.Name = xml.Document.Descendants().FirstOrDefault(x => x.Name.LocalName == "Name")?.Value ?? "";

                        tcx.Trackpoints = xml.Document.Descendants().Where(x => x.Name.LocalName == "Trackpoint").Select(s => new TrackPoint
                        {
                                Time = s.Descendants().FirstOrDefault(x => x.Name.LocalName == "Time")?.Value ?? "",
                                Position = new TcxPosition
                                {
                                    Lat = s.Descendants().FirstOrDefault(x => x.Name.LocalName == "LatitudeDegrees")?.Value ?? "",
                                    Long = s.Descendants().FirstOrDefault(x => x.Name.LocalName == "LongitudeDegrees")?.Value ?? "",
                                },
                                Altitude = s.Descendants().FirstOrDefault(x => x.Name.LocalName == "AltitudeMeters")?.Value ?? ""
                        })
                            .ToList();

                        return Ok(new BasicHttpResponse<GeoJson>() { Ok = true, Data = tcx.ToGeoJson(entry.EntryName) });
                    }

                    throw new Exception("Route data type not correct!");
                }

                throw new Exception("Nothing found!");

            }
            catch (Exception e)
            {
                return Ok(new BasicHttpResponse<object>(){Ok = false, Message = $"Error: {Shared.Helpers.Functions.ExceptionMessage(e)}" });
            }
        }



        [AllowAnonymous]
        [HttpGet("[action]/{id}")]
        public IActionResult DeleteItem(int id)
        {
            try
            {
                LeaderboardItem blog = _context.Leaderboard.FirstOrDefault(b => b.Id == id);

                if (blog == null)
                    throw new Exception($"No blog found! ID: {id}");

                else
                {
                    _context.Remove(blog);
                    _context.SaveChanges();

                    return Ok(new BasicHttpResponse<object> { Ok = true });
                }
            }
            catch (Exception e)
            {
                return Ok(new BasicHttpResponse<object> { Ok = false, Message = e.Message });
            }
        }

    }
}
