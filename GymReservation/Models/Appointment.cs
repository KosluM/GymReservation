using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace GymReservation.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = null!;
        public IdentityUser? User { get; set; }

        [Required]
        [Display(Name = "Antrenör")]
        public int TrainerId { get; set; }

        [Required]
        [Display(Name = "Hizmet")]
        public int GymServiceId { get; set; }

        [Display(Name = "Başlangıç Zamanı")]
        public DateTime StartDateTime { get; set; }

        [Display(Name = "Süre (dk)")]
        public int DurationMinutes { get; set; }

        [Display(Name = "Ücret")]
        public decimal Price { get; set; }

        [Display(Name = "Durum")]
        public string Status { get; set; } = "Beklemede";

        public Trainer? Trainer { get; set; }
        public GymService? GymService { get; set; }
    }
}
