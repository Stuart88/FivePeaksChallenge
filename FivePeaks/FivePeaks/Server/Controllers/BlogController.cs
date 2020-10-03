using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public class BlogController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly FivePeaksDbContext _context;

        public BlogController(IWebHostEnvironment environment)
        {
            _environment = environment;
            _context = new FivePeaksDbContext();
        }

        [AllowAnonymous]
        [HttpGet("[action]/{page}")]
        public IActionResult GetBlogs(int page = 0)
        {
            try
            {
                var blogs = _context.Blogs.Select(blog => new BlogPost
                {
                    Id = blog.Id,
                    CreationDate = blog.CreationDate,
                    Active = blog.Active,
                    Author = blog.Author,
                    //Content = blog.Content,
                    PostDate = blog.PostDate,
                    Title = blog.Title,
                    Views = blog.Views
                })
                    .OrderByDescending(b => b.PostDate)
                    .ToList();

                return Ok(new BasicHttpResponse<List<BlogPost>> {Ok = true, Data = blogs});
            }
            catch (Exception e)
            {
                return Ok(new BasicHttpResponse<object> { Ok = false, Message = e.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("[action]")]
        public IActionResult GetLiveBlogs()
        {
            try
            {
                var blogs = _context.Blogs.Where(b => b.Active && b.PostDate <= DateTime.Now)
                    .Select(blog => new BlogPost
                    {
                        Id = blog.Id,
                        CreationDate = blog.CreationDate,
                        Active = blog.Active,
                        Author = blog.Author,
                        PostDate = blog.PostDate,
                        Title = blog.Title,
                        Views = blog.Views
                    })
                    .OrderByDescending(b => b.PostDate)
                    .ToList();

                return Ok(new BasicHttpResponse<List<BlogPost>> { Ok = true, Data = blogs });
            }
            catch (Exception e)
            {
                return Ok(new BasicHttpResponse<object> { Ok = false, Message = e.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("[action]/{id}")]
        public IActionResult GetBlog(int id)
        {
            try
            {
                BlogPost blog = _context.Blogs.FirstOrDefault(b => b.Id == id);

                if (blog == null)
                    throw new Exception($"No blog found! ID: {id}");

                else
                {
                    blog.HeaderImageBase64 = "";

                    return Ok(new BasicHttpResponse<BlogPost> { Ok = true, Data = blog });
                }
            }
            catch (Exception e)
            {
                return Ok(new BasicHttpResponse<object> { Ok = false, Message = e.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("[action]/{id}")]
        public IActionResult IncrementViews(int id)
        {
            try
            {
                BlogPost blog = _context.Blogs.FirstOrDefault(b => b.Id == id);

                if (blog == null)
                    throw new Exception($"No blog found! ID: {id}");

                else
                {
                    blog.Views +=1;
                    _context.Entry(blog).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _context.SaveChanges();

                    return Ok(new BasicHttpResponse<object> { Ok = true });
                }
            }
            catch (Exception e)
            {
                return Ok(new BasicHttpResponse<object> { Ok = false, Message = e.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("[action]/{id}")]
        public IActionResult ToggleActive(int id)
        {
            try
            {
                BlogPost blog = _context.Blogs.FirstOrDefault(b => b.Id == id);

                if (blog == null)
                    throw new Exception($"No blog found! ID: {id}");

                else
                {
                    blog.Active = !blog.Active;
                    _context.Entry(blog).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _context.SaveChanges();

                    return Ok(new BasicHttpResponse<BlogPost> { Ok = true, Data = blog });
                }
            }
            catch (Exception e)
            {
                return Ok(new BasicHttpResponse<object> { Ok = false, Message = e.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("[action]/{id}")]
        public IActionResult DeleteBlog(int id)
        {
            try
            {
                BlogPost blog = _context.Blogs.FirstOrDefault(b => b.Id == id);

                if (blog == null)
                    throw new Exception($"No blog found! ID: {id}");

                else
                {
                    _context.Remove(blog);
                    _context.SaveChanges();

                    return Ok(new BasicHttpResponse<object> {Ok = true});
                }
            }
            catch (Exception e)
            {
                return Ok(new BasicHttpResponse<object> { Ok = false, Message = e.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("[action]/{id}")]
        public IActionResult GetHeaderImage(int id)
        {
            BlogPost blog = _context.Blogs.FirstOrDefault(b => b.Id == id);

            if (blog == null || string.IsNullOrEmpty(blog.HeaderImageBase64))
                return File("/images/backgrounds/grey.jpg", "image/jpeg", "grey.jpg");
            
            return File(Convert.FromBase64String(blog.HeaderImageBase64), "image/jpeg", $"{blog.Title}.jpg");
        }

        //[AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<IActionResult> AddEditBlog([FromBody] BlogPost blog)
        {
            try
            {
                if (blog == null)
                    throw new Exception("No blog posted!");

                BlogPost editing = _context.Blogs.FirstOrDefault(b => b.Id == blog.Id);

                if (editing != null)
                {
                    editing.Active = blog.Active;
                    editing.Author = blog.Author;
                    editing.Content = blog.Content;
                    editing.PostDate = blog.PostDate;
                    editing.Tags = blog.Tags;
                    editing.Title = blog.Title;
                    editing.Views = blog.Views;
                    _context.Entry(editing).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                }
                else
                {
                    editing = new BlogPost
                    {
                        Active = blog.Active,
                        Author = blog.Author,
                        Content = blog.Content,
                        PostDate = blog.PostDate,
                        Tags = blog.Tags,
                        Title = blog.Title,
                        Views = blog.Views
                    };

                    await _context.Blogs.AddAsync(editing);
                }

                //Header image added/replaced.
                if (blog.HeaderImageBase64.Length > 0)
                {
                    Image img = Image.Load(Convert.FromBase64String(blog.HeaderImageBase64));

                    JpegEncoder encoder = new JpegEncoder();

                    await using MemoryStream mem = new MemoryStream();

                    encoder.Quality = 100;

                    await img.SaveAsJpegAsync(mem, encoder);

                    int q = 100;
                    while (mem.Length >= 400000 && q > 1)
                    {
                        q -= 10;
                        encoder.Quality = q;
                        mem.Position = 0;
                        img = await Image.LoadAsync(mem);
                    
                        //start again
                        mem.SetLength(0);
                        await img.SaveAsJpegAsync(mem, encoder);
                    }

                    editing.HeaderImageBase64 = Convert.ToBase64String(mem.ToArray());

                   
                    if (mem.Length > 500000)
                        throw new Exception($"Could not compress image enough! Try uploading a smaller image");

                }
                    
                await _context.SaveChangesAsync();

                return Ok(new BasicHttpResponse<object> { Ok = true });


            }
            catch (Exception e)
            {
                return Ok(new BasicHttpResponse<object> { Ok = false, Message = e.Message });
            }
        }

    }
}
