using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using FivePeaks.Server.Database;
using FivePeaks.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace FivePeaks.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly FivePeaksDbContext _context;
        private const string AdminPass = "passpo"; 

        public AdminController(IWebHostEnvironment environment)
        {
            _environment = environment;
            _context = new FivePeaksDbContext();
        }

        [AllowAnonymous]
        [HttpPost("[action]")]
        public IActionResult Login()
        {
            try
            {
                string authHeader = Request.Headers["Authorization"];

                if (authHeader != null && authHeader.StartsWith("Basic"))
                {
                    string encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                    Encoding encoding = Encoding.GetEncoding("iso-8859-1");
                    string usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));

                    int seperatorIndex = usernamePassword.IndexOf(':');

                    string username = usernamePassword.Substring(0, seperatorIndex);
                    string password = usernamePassword.Substring(seperatorIndex + 1);

                    if (!string.IsNullOrEmpty(password) && password == AdminPass)
                    {
                        AdminUser loggingIn = _context.Admins.FirstOrDefault(a => a.Username == username);

                        if (loggingIn == null)
                            return Ok(new BasicHttpResponse<AdminUser> {Message = "User not found!"});
                        else
                        {
                            loggingIn.Password = "";
                            return Ok(new BasicHttpResponse<AdminUser> {Ok = true, Data = loggingIn});
                        }
                    }
                    else
                        throw new Exception("Incorrect username or password!");
                }
                else
                    throw new Exception("The authorization header is either empty or isn't Basic.");
            }
            catch (Exception e)
            {
                return Ok(new BasicHttpResponse<AdminUser> { Message = e.Message });
            }
        }

    }
}
