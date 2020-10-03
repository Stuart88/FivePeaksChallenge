using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;
using FivePeaks.Server.Database;
using FivePeaks.Shared.Models;

namespace FivePeaks.Server.Pages
{
    public class BaseIndexModel : PageModel
    {
        private readonly FivePeaksDbContext _context;

        public string ShareImageUrl { get; set; } = "https://thefivepeakschallenge.co.uk/images/icons/logo-words.png";
        public string ShareTitle { get; set; } = "The Five Peaks Challenge";
        public string ShareDescription { get; set; } = "Yorkshire Dales newest and most exclusive challenge";
        public string ShareUrl { get; set; } = "https://thefivepeakschallenge.co.uk";

        public BaseIndexModel()
        {
            _context = new FivePeaksDbContext();
        }

        public void OnGet()
        {
            this.ShareUrl = this.ShareUrl + Request.Path.Value;

            if (Request.Path.Value.StartsWith("/blog") && Request.Path.Value.Split("/").Length > 2)
            {
                // Path of the form "/blog/4/Testingggg1-No-1"

                int blogId = int.Parse(Request.Path.Value.Split("/")[2]);

                BlogPost blog = _context.Blogs.Find(blogId);

                if (blog != null)
                {
                    this.ShareImageUrl = $"https://thefivepeakschallenge.co.uk/Blog/GetHeaderImage/{blog.Id}";
                    this.ShareTitle = blog.Title;
                    this.ShareDescription = "The Five Peaks - Adventure Blog";
                }

            }
            
        }
    }
}
