using GymReservation.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GymReservation.Data
{
    // Artık IdentityUser değil, ApplicationUser kullanıyoruz
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Bizim tablolar
        public DbSet<FitnessCenter> FitnessCenters { get; set; } = null!;
        public DbSet<GymService> GymServices { get; set; } = null!;
        public DbSet<Trainer> Trainers { get; set; } = null!;
        public DbSet<TrainerService> TrainerServices { get; set; } = null!;
        public DbSet<TrainerAvailability> TrainerAvailabilities { get; set; } = null!;
        public DbSet<Appointment> Appointments { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

     
          
            builder.Entity<Trainer>()
                .HasOne(t => t.FitnessCenter)
                .WithMany() 
                .HasForeignKey(t => t.FitnessCenterId)
                .OnDelete(DeleteBehavior.Restrict); 
        }
    }
}
