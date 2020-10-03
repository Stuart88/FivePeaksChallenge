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
    public class AccountsController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly FivePeaksDbContext _context;
        private const string AdminPass = "passpo"; 

        public AccountsController(IWebHostEnvironment environment)
        {
            _environment = environment;
            _context = new FivePeaksDbContext();
        }

        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<IActionResult> UpdateUser([FromBody] UserAccount user)
        {
            try
            {
                var u = _context.Users.FirstOrDefault(x => x.Email == user.Email);

                DateTime now = DateTime.Now;

                if (u == null)
                {
                    // New user. Assign user name and save.
                    int idNum = _context.Users.Any()
                        ? _context.Users.Max(x => x.Id) + 1000
                        : 1000;

                    u = new UserAccount
                    {
                        Username = $"FivePeaker_{idNum}", 
                        Email = user.Email, 
                        CreationDate = now, 
                        LastLogin = now
                    };

                    await _context.Users.AddAsync(u);
                }
                else
                {
                    u.LastLogin = now;

                    if (!string.IsNullOrEmpty(user.Username) && user.Username != u.Username)
                    {
                        //User is setting custom username
                        if (_context.Users.Any(x => x.Username == user.Username))
                            throw new Exception("Username is taken!");

                        if(user.Username.Length < 3)
                            throw new Exception("Username too short!");

                        if (user.Username.Length >= 30)
                            throw new Exception("Username too long!");
                        
                        u.Username = user.Username;
                    }

                    _context.Entry(u).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                }

                await _context.SaveChangesAsync();

                return Ok(new BasicHttpResponse<UserAccount> { Ok = true, Data = u });
            }
            catch (Exception e)
            {
                return Ok(new BasicHttpResponse<UserAccount> { Ok = false, Message = e.Message });
            }
        }

    }
}
