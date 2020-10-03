using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using FivePeaks.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace FivePeaks.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SiteController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;

        public SiteController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [AllowAnonymous]
        [HttpGet("[action]/{key}")]
        public IActionResult GetApiKey(string key)
        {
            bool ok = Request.Host.Host.Contains("thefivepeakschallenge");
#if DEBUG
            ok = true;
#endif
            if (!ok)
                return Unauthorized();  

            return key switch
            {
                "OSMap" => new JsonResult(ApiKeys.OSMapsAPIKey),
                _ => Unauthorized()
            };
        }

        [AllowAnonymous]
        [HttpGet("[action]/{fileType}")]
        public IActionResult GetRouteData(string fileType)
        {
            return File($"/downloads/The_Five_Peaks_Challenge.{fileType}", "application/octet-stream", $"The_Five_Peaks_Challenge.{fileType}");
        }

        [AllowAnonymous]
        [HttpGet("[action]/{path}")]
        public IActionResult GetImage(string path)
        {
            path = System.Web.HttpUtility.UrlDecode(path);

            string mimeType;
            
            if (path.EndsWith(".svg"))
            {
                mimeType = "image/svg+xml";
            }
            else if (path.EndsWith(".gif"))
            {
                mimeType = "image/gif";
            }
            else
            {
                mimeType = "image/png";
            }

            
            return File($"/images/{path}", mimeType, $"{path}");
        }

        [AllowAnonymous]
        [HttpGet("[action]/{path}")]
        public IActionResult GetImageCompressed(string path)
        {

            path = System.Web.HttpUtility.UrlDecode(path);

            string mimeType;

            if (path.EndsWith(".svg"))
            {
                mimeType = "image/svg+xml";
            }
            else if (path.EndsWith(".gif"))
            {
                mimeType = "image/gif";
            }
            else
            {
                mimeType = "image/png";
            }

            return File($"/images-compressed/{path}", mimeType, $"{path}");
        }
    }
}
