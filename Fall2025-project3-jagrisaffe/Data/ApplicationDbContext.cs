using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Fall2025_project3_jagrisaffe.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Fall2025_project3_jagrisaffe.Models.Actor> Actor { get; set; } = default!;
        public DbSet<Fall2025_project3_jagrisaffe.Models.Movie> Movie { get; set; } = default!;
        public DbSet<Fall2025_project3_jagrisaffe.Models.ActorMovie> ActorMovie { get; set; } = default!;
    }
}