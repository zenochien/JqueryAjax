using Jquery_Ajax.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Jquery_Ajax.DbContent
{
    public class MovieDbContext : DbContext
    {
        private readonly IConfigurationRoot _config;
        public MovieDbContext(IConfigurationRoot config, DbContextOptions options)
            : base(options)
        {
            _config = config;
        }

        public DbSet<Movie> Movies { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(_config["ConnectionStrings:MvcMovieContext"]); //connection string get by appsetting.json
        }

    }
}
