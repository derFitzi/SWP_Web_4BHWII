using Microsoft.EntityFrameworkCore;
using WebProjekt.Models;

namespace WebProjekt.Services
{
    public class DbManager : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Article> Articles { get; set; }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderArticle> OrderArticles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = "Server=localhost;database=web_4b_g1;user=root;password=root";
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }
        }
}
