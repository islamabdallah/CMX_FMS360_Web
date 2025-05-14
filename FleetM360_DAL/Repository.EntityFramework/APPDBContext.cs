using FleetM360_DAL.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FleetM360_DAL.Repository.EntityFramework
{
    public class APPDBContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public APPDBContext(DbContextOptions<APPDBContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            //base.OnModelCreating(builder);
            //builder.Entity<ApplicationUser>();
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }


    }
}
