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
                    .ThenInclude(t => t.FitnessCenter)
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
                    .ThenInclude(t => t.FitnessCenter)
                .Include(a => a.GymService)
                .Include(a => a.User)
                .OrderByDescending(a => a.StartDateTime)
                .ToListAsync();

            return View(list);
        }

 
        // ADIM 0: SALON SEÇ

        public async Task<IActionResult> SelectFitnessCenter()
        {
            var centers = await _context.FitnessCenters.ToListAsync();
            return View(centers);
        }

    
        // ADIM 1: HİZMET SEÇ
     
        public async Task<IActionResult> SelectService(int fitnessCenterId)
        {
            var services = await _context.GymServices
                .Include(s => s.FitnessCenter)
                .Where(s => s.FitnessCenterId == fitnessCenterId)
                .ToListAsync();

            ViewBag.FitnessCenterId = fitnessCenterId;
            return View(services);
        }

        // ADIM 2: ANTRENÖR SEÇ 
      
        public async Task<IActionResult> SelectTrainer(int serviceId)
        {
            var service = await _context.GymServices
                .Include(s => s.FitnessCenter)
                .FirstOrDefaultAsync(s => s.Id == serviceId);

            if (service == null) return NotFound();

            var trainers = await _context.TrainerServices
                .Where(ts => ts.GymServiceId == serviceId)
                .Select(ts => ts.Trainer!)
                .Where(t => t.FitnessCenterId == service.FitnessCenterId)
                .Distinct()
                .ToListAsync();

            ViewBag.SelectedService = service;
            return View(trainers);
        }

    
        // ADIM 3: TARİH / SAAT SEÇ
   
        public async Task<IActionResult> SelectTime(int serviceId, int trainerId)
        {
            var service = await _context.GymServices
                .Include(s => s.FitnessCenter)
                .FirstOrDefaultAsync(s => s.Id == serviceId);

            var trainer = await _context.Trainers
                .Include(t => t.FitnessCenter)
                .FirstOrDefaultAsync(t => t.Id == trainerId);

            if (service == null || trainer == null) return NotFound();

        
            if (service.FitnessCenterId != trainer.FitnessCenterId)
                return BadRequest("Salon uyumsuzluğu.");

            var availabilities = await _context.TrainerAvailabilities
                .Where(a => a.TrainerId == trainerId && a.Date >= DateTime.Today)
                .OrderBy(a => a.Date)
                .ThenBy(a => a.StartTime)
                .ToListAsync();

            ViewBag.SelectedService = service;
            ViewBag.SelectedTrainer = trainer;

            return View(availabilities);
        }

     
        // POST: MÜSAİTLİKTEN RANDEVU OLUŞTUR
     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFromAvailability(int availabilityId, int serviceId, int trainerId)
        {
            var availability = await _context.TrainerAvailabilities
                .FirstOrDefaultAsync(a => a.Id == availabilityId);

            var service = await _context.GymServices.FirstOrDefaultAsync(s => s.Id == serviceId);
            var trainer = await _context.Trainers.FirstOrDefaultAsync(t => t.Id == trainerId);

            if (availability == null || service == null || trainer == null)
                return NotFound();

            // Salon tutarlılığı
            if (service.FitnessCenterId != trainer.FitnessCenterId)
                return BadRequest("Salon uyumsuzluğu.");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Challenge();

            var start = availability.Date.Date + availability.StartTime;
            var duration = (int)(availability.EndTime - availability.StartTime).TotalMinutes;
            if (duration <= 0)
                duration = service.DurationMinutes;

            var end = start.AddMinutes(duration);

            // Antrenör çakışma
            var trainerConflict = await _context.Appointments
                .AnyAsync(a =>
                    a.TrainerId == trainerId &&
                    a.Status != "İptal" &&
                    a.StartDateTime < end &&
                    a.StartDateTime.AddMinutes(a.DurationMinutes) > start);

            if (trainerConflict)
            {
                TempData["Error"] = "Bu zaman diliminde antrenörün başka randevusu var.";
                return RedirectToAction(nameof(SelectTime), new { serviceId, trainerId });
            }

            // Kullanıcı çakışma
            var userConflict = await _context.Appointments
                .AnyAsync(a =>
                    a.UserId == userId &&
                    a.Status != "İptal" &&
                    a.StartDateTime < end &&
                    a.StartDateTime.AddMinutes(a.DurationMinutes) > start);

            if (userConflict)
            {
                TempData["Error"] = "Bu zaman diliminde başka randevunuz var.";
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

   
        // ADMIN: DURUM DEĞİŞTİR
     
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int id, string status)
        {
            if (status != "Beklemede" && status != "Onaylandı" && status != "İptal")
                return BadRequest();

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return NotFound();

            appointment.Status = status;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(AdminIndex));
        }
    }
}
