using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GymReservation.Data;
using GymReservation.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymReservation.Controllers
{
    [Authorize]
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AppointmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // KULLANICI: Kendi randevularım
        public async Task<IActionResult> MyAppointments()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var list = await _context.Appointments
                .Where(a => a.UserId == userId)
                .Include(a => a.Trainer)
                .Include(a => a.GymService)
                .OrderByDescending(a => a.StartDateTime)
                .ToListAsync();

            return View(list);
        }

        // ADMIN: tüm randevular
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminIndex()
        {
            var list = await _context.Appointments
                .Include(a => a.Trainer)
                .Include(a => a.GymService)
                .Include(a => a.User)
                .OrderByDescending(a => a.StartDateTime)
                .ToListAsync();

            return View(list);
        }

        // ADIM 1: Hizmet seç
        public async Task<IActionResult> SelectService()
        {
            ViewBag.CurrentStep = 1;

            var services = await _context.GymServices
                .Include(s => s.FitnessCenter)
                .ToListAsync();

            return View(services);
        }

        // ADIM 2: Hizmeti verebilen antrenörü seç
        public async Task<IActionResult> SelectTrainer(int serviceId)
        {
            ViewBag.CurrentStep = 2;

            var service = await _context.GymServices
                .Include(s => s.FitnessCenter)
                .FirstOrDefaultAsync(s => s.Id == serviceId);

            if (service == null) return NotFound();

            var trainers = await _context.TrainerServices
                .Where(ts => ts.GymServiceId == serviceId)
                .Select(ts => ts.Trainer!)
                .Distinct()
                .ToListAsync();

            ViewBag.SelectedService = service;
            return View(trainers);
        }

        // ADIM 3: Tarih / saat seç
        public async Task<IActionResult> SelectTime(int serviceId, int trainerId)
        {
            ViewBag.CurrentStep = 3;

            var service = await _context.GymServices.FindAsync(serviceId);
            var trainer = await _context.Trainers.FindAsync(trainerId);

            if (service == null || trainer == null) return NotFound();

            var availabilities = await _context.TrainerAvailabilities
                .Where(a => a.TrainerId == trainerId && a.Date >= DateTime.Today)
                .OrderBy(a => a.Date)
                .ThenBy(a => a.StartTime)
                .ToListAsync();

            ViewBag.SelectedService = service;
            ViewBag.SelectedTrainer = trainer;

            return View(availabilities);
        }

        // POST: seçilen müsaitlikten randevu oluştur
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFromAvailability(int availabilityId, int serviceId, int trainerId)
        {
            var availability = await _context.TrainerAvailabilities
                .FirstOrDefaultAsync(a => a.Id == availabilityId);

            var service = await _context.GymServices.FirstOrDefaultAsync(s => s.Id == serviceId);
            if (availability == null || service == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Challenge(); // login değilse

            var start = availability.Date.Date + availability.StartTime;
            var duration = (int)(availability.EndTime - availability.StartTime).TotalMinutes;
            if (duration <= 0)
            {
                duration = service.DurationMinutes;
            }
            var end = start.AddMinutes(duration);

            // Antrenör için çakışma kontrolü
            var hasConflictForTrainer = await _context.Appointments
                .AnyAsync(a => a.TrainerId == trainerId &&
                               a.StartDateTime < end &&
                               a.StartDateTime.AddMinutes(a.DurationMinutes) > start);

            if (hasConflictForTrainer)
            {
                TempData["Error"] = "Bu zaman diliminde antrenörün başka randevusu var.";
                return RedirectToAction(nameof(SelectTime), new { serviceId, trainerId });
            }

            // Kullanıcı için çakışma kontrolü
            var hasConflictForUser = await _context.Appointments
                .AnyAsync(a => a.UserId == userId &&
                               a.StartDateTime < end &&
                               a.StartDateTime.AddMinutes(a.DurationMinutes) > start);

            if (hasConflictForUser)
            {
                TempData["Error"] = "Bu zaman diliminde başka bir randevunuz var.";
                return RedirectToAction(nameof(SelectTime), new { serviceId, trainerId });
            }

            var appointment = new Appointment
            {
                UserId = userId,
                TrainerId = trainerId,
                GymServiceId = serviceId,
                StartDateTime = start,
                DurationMinutes = duration,
                Price = service.Price,
                Status = "Beklemede"
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Randevunuz oluşturuldu.";
            return RedirectToAction(nameof(MyAppointments));
        }

        // ADMIN: durum değiştirme (Onayla / İptal et)
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int id, string status)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return NotFound();

            appointment.Status = status;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(AdminIndex));
        }
    }
}
