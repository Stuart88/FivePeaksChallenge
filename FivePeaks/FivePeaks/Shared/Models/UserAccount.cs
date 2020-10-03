using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace FivePeaks.Shared.Models
{
    public class UserAccount
    {
        [PrimaryKey, AutoIncrement]
        [Column("Id")]
        public int Id { get; set; }

        [Column("Username")] public string Username { get; set; }

        [Column("Email")] public string Email { get; set; }

        [Column("CreationDate")] public DateTime CreationDate { get; set; } = DateTime.Now;

        [Column("LastLogin")] public DateTime LastLogin { get; set; } = DateTime.Now;

    }
}
