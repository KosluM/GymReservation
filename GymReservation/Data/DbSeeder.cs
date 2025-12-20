using GymReservation.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GymReservation.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

           
            await context.Database.MigrateAsync();

       
        
            if (context.FitnessCenters.Any())
                return;

         
            var seedStartDate = new DateTime(2025, 12, 24);

      
            var center1 = new FitnessCenter
            {
                Name = "Sakarya Fitness Center",
                Address = "Serdivan / Sakarya",
                OpeningTime = new TimeSpan(9, 0, 0),
                ClosingTime = new TimeSpan(23, 0, 0)
            };

            var center2 = new FitnessCenter
            {
                Name = "Serdivan Power Gym",
                Address = "Serdivan AVM Yakını",
                OpeningTime = new TimeSpan(8, 0, 0),
                ClosingTime = new TimeSpan(22, 0, 0)
            };

            context.FitnessCenters.AddRange(center1, center2);
            await context.SaveChangesAsync();

            // -------------------------
            // 2️⃣ SERVICES
            // -------------------------
            var servicesList = new List<GymService>
            {
                // Center1
                new GymService { Name = "Fitness PT", DurationMinutes = 60, Price = 500, FitnessCenterId = center1.Id },
                new GymService { Name = "Yoga", DurationMinutes = 60, Price = 350, FitnessCenterId = center1.Id },
                new GymService { Name = "Pilates", DurationMinutes = 50, Price = 400, FitnessCenterId = center1.Id },

                // Center2
                new GymService { Name = "CrossFit", DurationMinutes = 45, Price = 450, FitnessCenterId = center2.Id },
                new GymService { Name = "Zumba", DurationMinutes = 50, Price = 300, FitnessCenterId = center2.Id },
                new GymService { Name = "Kilo Verme Programı", DurationMinutes = 45, Price = 380, FitnessCenterId = center2.Id },
            };

            context.GymServices.AddRange(servicesList);
            await context.SaveChangesAsync();

            // -------------------------
            // 3️⃣ TRAINERS
            // -------------------------
            var trainers = new List<Trainer>
            {
                // Center1
                new Trainer
                {
                    FullName = "Ahmet Yılmaz",
                    Specialty = "Kas Geliştirme",
                    Bio = "10 yıllık deneyimli personal trainer.",
                    FitnessCenterId = center1.Id
                },
                new Trainer
                {
                    FullName = "Elif Kaya",
                    Specialty = "Yoga & Pilates",
                    Bio = "Yoga ve pilates uzmanı.",
                    FitnessCenterId = center1.Id
                },

                // Center2
                new Trainer
                {
                    FullName = "Mehmet Demir",
                    Specialty = "Kilo Verme",
                    Bio = "Yağ yakımı ve kondisyon.",
                    FitnessCenterId = center2.Id
                },
                new Trainer
                {
                    FullName = "Zeynep Acar",
                    Specialty = "Zumba & Cardio",
                    Bio = "Grup dersleri uzmanı.",
                    FitnessCenterId = center2.Id
                }
            };

            context.Trainers.AddRange(trainers);
            await context.SaveChangesAsync();

            // -------------------------
            // 4️⃣ TRAINER - SERVICE
            // ✅ Salon uyumu korunuyor (Center1 hizmetleri -> Center1 antrenörleri, Center2 -> Center2)
            // -------------------------
            context.TrainerServices.AddRange(
                // Center1: Fitness PT / Yoga / Pilates -> (Ahmet + Elif)
                new TrainerService { TrainerId = trainers[0].Id, GymServiceId = servicesList[0].Id }, // Ahmet - Fitness PT
                new TrainerService { TrainerId = trainers[1].Id, GymServiceId = servicesList[0].Id }, // Elif  - Fitness PT

                new TrainerService { TrainerId = trainers[0].Id, GymServiceId = servicesList[1].Id }, // Ahmet - Yoga
                new TrainerService { TrainerId = trainers[1].Id, GymServiceId = servicesList[1].Id }, // Elif  - Yoga

                new TrainerService { TrainerId = trainers[0].Id, GymServiceId = servicesList[2].Id }, // Ahmet - Pilates
                new TrainerService { TrainerId = trainers[1].Id, GymServiceId = servicesList[2].Id }, // Elif  - Pilates

                // Center2: CrossFit / Zumba / Kilo Verme -> (Mehmet + Zeynep)
                new TrainerService { TrainerId = trainers[2].Id, GymServiceId = servicesList[3].Id }, // Mehmet - CrossFit
                new TrainerService { TrainerId = trainers[3].Id, GymServiceId = servicesList[3].Id }, // Zeynep - CrossFit

                new TrainerService { TrainerId = trainers[2].Id, GymServiceId = servicesList[4].Id }, // Mehmet - Zumba
                new TrainerService { TrainerId = trainers[3].Id, GymServiceId = servicesList[4].Id }, // Zeynep - Zumba

                new TrainerService { TrainerId = trainers[2].Id, GymServiceId = servicesList[5].Id }, // Mehmet - Kilo Verme
                new TrainerService { TrainerId = trainers[3].Id, GymServiceId = servicesList[5].Id }  // Zeynep - Kilo Verme
            );

            await context.SaveChangesAsync();

            // -------------------------
            // 5️⃣ AVAILABILITIES
            // -------------------------
            var availabilities = new List<TrainerAvailability>();

            foreach (var trainer in trainers)
            {
                for (int i = 0; i < 14; i++)
                {
                    var date = seedStartDate.AddDays(i);

                    // Sabah bloğu
                    availabilities.Add(new TrainerAvailability
                    {
                        TrainerId = trainer.Id,
                        Date = date,
                        StartTime = new TimeSpan(10, 0, 0),
                        EndTime = new TimeSpan(12, 0, 0)
                    });

                    // Öğleden sonra bloğu
                    availabilities.Add(new TrainerAvailability
                    {
                        TrainerId = trainer.Id,
                        Date = date,
                        StartTime = new TimeSpan(14, 0, 0),
                        EndTime = new TimeSpan(17, 0, 0)
                    });
                }
            }

            context.TrainerAvailabilities.AddRange(availabilities);
            await context.SaveChangesAsync();

            // -------------------------
            // 6️⃣ DEMO USERS
            // -------------------------
            await CreateUserIfNotExists(userManager,
                "user1@test.com", "Test123*", "Ali", "Kaya");

            await CreateUserIfNotExists(userManager,
                "user2@test.com", "Test123*", "Ayşe", "Demir");

            // -------------------------
            // 7️⃣ DEMO RANDEVULAR 
            // -------------------------
            var user1 = await userManager.FindByEmailAsync("user1@test.com");
            var user2 = await userManager.FindByEmailAsync("user2@test.com");

            // Randevuları sadece yoksa ekle
            if (!context.Appointments.Any())
            {
                // 1) user1 -> Center1: Fitness PT -> Ahmet 
                if (user1 != null)
                {
                    context.Appointments.Add(new Appointment
                    {
                        UserId = user1.Id,
                        TrainerId = trainers[0].Id,             
                        GymServiceId = servicesList[0].Id,       
                        StartDateTime = new DateTime(2025, 12, 24, 10, 0, 0),
                        DurationMinutes = servicesList[0].DurationMinutes,
                        Price = servicesList[0].Price,
                        Status = "Onaylandı"
                    });
                }

                // 2) user2 -> Center2: Zumba -> Zeynep
                if (user2 != null)
                {
                    context.Appointments.Add(new Appointment
                    {
                        UserId = user2.Id,
                        TrainerId = trainers[3].Id,              
                        GymServiceId = servicesList[4].Id,      
                        StartDateTime = new DateTime(2025, 12, 25, 14, 0, 0),
                        DurationMinutes = servicesList[4].DurationMinutes,
                        Price = servicesList[4].Price,
                        Status = "Beklemede"
                    });
                }

                await context.SaveChangesAsync();
            }
        }

        private static async Task CreateUserIfNotExists(
            UserManager<ApplicationUser> userManager,
            string email,
            string password,
            string firstName,
            string lastName)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user != null) return;

            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FirstName = firstName,
                LastName = lastName
            };

            await userManager.CreateAsync(user, password);
        }
    }
}
