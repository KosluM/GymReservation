using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using GymReservation.Models;

namespace GymReservation.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
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
    }
}
