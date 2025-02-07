using CustomUserLogin.ViewModel;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CustomUserLogin.Models;

namespace CustomUserLogin.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<EntityRoleType> EntityRoleType { get; set; } = default!;
        public DbSet<Employers> Employers { get; set; } = default!;
        public virtual DbSet<EmployerDefaulter> EmployerDefaulters { get; set; }
        public DbSet<PaymentPlan> PaymentPlans { get; set; } 
        public DbSet<PaymentDetails> PaymentDetails { get; set; } 

    }
}
