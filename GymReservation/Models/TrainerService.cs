namespace GymReservation.Models
{
    public class TrainerService
    {
        public int Id { get; set; }

        public int TrainerId { get; set; }
        public int GymServiceId { get; set; }

        public Trainer? Trainer { get; set; }
        public GymService? GymService { get; set; }
    }
}
