using System;
using System.ComponentModel.DataAnnotations;

namespace GymReservation.Models
{
    public class TrainerAvailability
    {
        public int Id { get; set; }

        [Display(Name = "Antrenör")]
        [Required]
        public int TrainerId { get; set; }

        [Required(ErrorMessage = "Tarih zorunludur.")]
        [DataType(DataType.Date)]
        [Display(Name = "Tarih")]
        public DateTime Date { get; set; }                // Hangi gün

        [Required(ErrorMessage = "Başlangıç saati zorunludur.")]
        [Display(Name = "Başlangıç Saati")]
        public TimeSpan StartTime { get; set; }           // Başlangıç saati

        [Required(ErrorMessage = "Bitiş saati zorunludur.")]
        [Display(Name = "Bitiş Saati")]
        public TimeSpan EndTime { get; set; }             // Bitiş saati

        public Trainer? Trainer { get; set; }
    }
}
