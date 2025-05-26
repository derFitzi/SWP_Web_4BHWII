using Microsoft.EntityFrameworkCore;
using WebProjekt.Models;

namespace WebProjekt.Services
{
    public class DbManager : DbContext
    {
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = "Server=localhost;database=web_4b_g1;user=root;password=root";
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }
        }
}
