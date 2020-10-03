using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FivePeaks.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace FivePeaks.Server.Database
{
    public class FivePeaksDbContext : DbContext
    {
        public DbSet<BlogPost> Blogs { get; set; }
        public DbSet<AdminUser> Admins { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=fivepeaks.db");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BlogPost>().ToTable("Blogs");
            modelBuilder.Entity<AdminUser>().ToTable("Admins");

            //modelBuilder.Entity<BlogPost>()
            //    .Property(e => e.Id)
            //    .ValueGeneratedOnAdd();

            //modelBuilder.Entity<AdminUser>()
            //    .Property(e => e.Id)
            //    .ValueGeneratedOnAdd();
        }
    }
}
