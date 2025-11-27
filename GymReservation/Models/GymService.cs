using System.ComponentModel.DataAnnotations;

namespace GymReservation.Models
{
    public class GymService
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Hizmet adı zorunludur.")]
        [StringLength(100)]
        [Display(Name = "Hizmet Adı")]
        public string Name { get; set; } = null!;

        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        [Range(1, 300, ErrorMessage = "Süre 1-300 dakika arasında olmalıdır.")]
        [Display(Name = "Hizmet Süresi (dk)")]
        public int DurationMinutes { get; set; }

        [Range(0, 10000)]
        [Display(Name = "Ücret")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Display(Name = "Salon")]
        public int FitnessCenterId { get; set; }

        public FitnessCenter? FitnessCenter { get; set; }
        public ICollection<TrainerService>? TrainerServices { get; set; }
        public ICollection<Appointment>? Appointments { get; set; }
    }
}
