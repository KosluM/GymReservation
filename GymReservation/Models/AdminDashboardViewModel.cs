using System;
using System.Collections.Generic;

namespace GymReservation.Models
{
    public class AdminDashboardViewModel
    {
        // Kartlardaki sayılar
        public int TotalUsers { get; set; }
        public int TotalTrainers { get; set; }
        public int TotalAppointments { get; set; }
        public int PendingAppointments { get; set; }
        public int TodayAppointmentsCount { get; set; }

        // Bugünkü randevular
        public List<Appointment> TodaysAppointments { get; set; } = new();

        // İleride lazımsa diye
        public DateTime Today { get; set; }
    }
}
