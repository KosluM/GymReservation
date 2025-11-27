using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GymReservation.Models
{
    public class FitnessCenter
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Salon adı zorunludur.")]
        [StringLength(100, ErrorMessage = "Salon adı en fazla 100 karakter olabilir.")]
        [Display(Name = "Salon Adı")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Adres zorunludur.")]
        [StringLength(200, ErrorMessage = "Adres en fazla 200 karakter olabilir.")]
        [Display(Name = "Adres")]
        public string Address { get; set; } = null!;

        [Required(ErrorMessage = "Açılış saati zorunludur.")]
        [Display(Name = "Açılış Saati")]
        public TimeSpan OpeningTime { get; set; }

        [Required(ErrorMessage = "Kapanış saati zorunludur.")]
        [Display(Name = "Kapanış Saati")]
        public TimeSpan ClosingTime { get; set; }

        // İlişkiler
        public ICollection<GymService>? Services { get; set; }
    }
}
