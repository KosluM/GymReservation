using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GymReservation.Models
{
    public class Trainer
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad soyad zorunludur.")]
        [StringLength(100)]
        [Display(Name = "Ad Soyad")]
        public string FullName { get; set; } = null!;

        [StringLength(100)]
        [Display(Name = "Uzmanlık Alanı")]
        public string? Specialty { get; set; }

        [Display(Name = "Biyografi")]
        [DataType(DataType.MultilineText)]
        public string? Bio { get; set; }

        // İlişkiler
        public ICollection<TrainerService>? TrainerServices { get; set; }
        public ICollection<TrainerAvailability>? Availabilities { get; set; }
        public ICollection<Appointment>? Appointments { get; set; }
    }
}
