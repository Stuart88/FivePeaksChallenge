﻿using System;
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
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

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

                    _context.Leaderboard.Attach(leaderboardItem);
                }

                await _context.SaveChangesAsync();

                return Ok(new BasicHttpResponse<object> { Ok = true});
            }
            catch (Exception e)
            {
                return Ok(new BasicHttpResponse<object> { Ok = false, Message = Shared.Helpers.Functions.ExceptionMessage(e) });
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
                var items = _context.Leaderboard
                    .Where(i => i.PostedByUserId == userId || userId == 0)
                    .Select(s => Helpers.Helpers.LeaderboardItemWithoutRouteData(s))
                    .ToList();
                
                return Ok(new BasicHttpResponse<List<LeaderboardItem>> { Ok = true, Data = items });
            }
            catch (Exception e)
            {
                return Ok(new BasicHttpResponse<object> { Ok = false, Message = e.Message });
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
