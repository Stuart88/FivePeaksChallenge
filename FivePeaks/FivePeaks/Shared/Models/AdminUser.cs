using SQLite;

namespace FivePeaks.Shared.Models
{
    public class AdminUser
    {
        [PrimaryKey, AutoIncrement]
        [Column("Id")]
        public int Id { get; set; }

        [Column("Username")]
        public string Username { get; set; }

        [Column("Password")]
        public string Password { get; set; }
    }
}
