using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShiftSchedularApplication.Models;

namespace ShiftSchedularApplication.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Shift> Shifts { get; set; }
        public DbSet<Availability> Availabilities { get; set; }
        public DbSet<PayStub> PayStubs { get; set; }
        public DbSet<Punch> Punches { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Add foreign key constraints manually, linking EmployeeId to AspNetUsers.Id (Identity)
            builder.Entity<Shift>()
                .HasOne<Microsoft.AspNetCore.Identity.IdentityUser>()
                .WithMany()
                .HasForeignKey(s => s.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Availability>()
                .HasOne<Microsoft.AspNetCore.Identity.IdentityUser>()
                .WithMany()
                .HasForeignKey(a => a.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PayStub>()
                .HasOne<Microsoft.AspNetCore.Identity.IdentityUser>()
                .WithMany()
                .HasForeignKey(p => p.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Punch>()
                .HasOne<Microsoft.AspNetCore.Identity.IdentityUser>()
                .WithMany()
                .HasForeignKey(p => p.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
