using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace GymReservation.Models
{
    public class FitnessAiRequestViewModel
    {
        [Required]
        [Display(Name = "Cinsiyet")]
        public string Gender { get; set; } = "Erkek"; // Erkek / Kadın

        [Required]
        [Range(10, 90)]
        [Display(Name = "Yaş")]
        public int Age { get; set; }

        [Required]
        [Range(100, 250)]
        [Display(Name = "Boy (cm)")]
        public int HeightCm { get; set; }

        [Required]
        [Range(30, 300)]
        [Display(Name = "Kilo (kg)")]
        public double WeightKg { get; set; }

        [Required]
        [Display(Name = "Hedef")]
        public string Goal { get; set; } = "Kilo vermek";

        [Required]
        [Display(Name = "Aktivite Seviyesi")]
        public string ActivityLevel { get; set; } = "Orta";

        [Display(Name = "Ek Bilgi (opsiyonel)")]
        public string? AdditionalInfo { get; set; }

       
        [Display(Name = "Referans Fotoğraf (opsiyonel)")]
        public IFormFile? Photo { get; set; }

       
        public string? ResultText { get; set; }
    }
}
