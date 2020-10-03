using System;
using SQLite;

namespace FivePeaks.Shared.Models
{
    public class BlogPost
    {
        [PrimaryKey, AutoIncrement]
        [Column("Id")]
        public int Id { get; set; }

        [Column("Title")] public string Title { get; set; } = "";

        [Column("Author")] public string Author { get; set; } = "";

        [Column("Tags")] public string Tags { get; set; } = "";

        [Column("Content")] public string Content { get; set; } = "";

        [Column("PostDate")] public DateTime PostDate { get; set; } = DateTime.Now;

        [Column("CreationDate")] public DateTime CreationDate { get; set; } = DateTime.Now;

        [Column("Active")] public bool Active { get; set; } = true;

        [Column("Views")] public int Views { get; set; } = 0;

        [Column("HeaderImageBase64")] public string HeaderImageBase64 { get; set; } = "";
    }
}
