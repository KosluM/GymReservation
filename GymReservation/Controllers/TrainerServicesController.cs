using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GymReservation.Data;
using GymReservation.Models;
using Microsoft.AspNetCore.Authorization;

namespace GymReservation.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TrainerServicesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TrainerServicesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TrainerServices/Manage/5
        public async Task<IActionResult> Manage(int id)
        {
            var trainer = await _context.Trainers.FindAsync(id);
            if (trainer == null)
                return NotFound();

            // Tüm hizmetleri al
            var services = await _context.GymServices.ToListAsync();

            // Antrenörün şimdiki hizmetleri
            var trainerServices = await _context.TrainerServices
                .Where(x => x.TrainerId == id)
                .Select(x => x.GymServiceId)
                .ToListAsync();

            ViewBag.Trainer = trainer;

            var model = services.Select(x => new TrainerServiceCheckboxViewModel
            {
                ServiceId = x.Id,
                ServiceName = x.Name,
                IsSelected = trainerServices.Contains(x.Id)
            }).ToList();

            return View(model);
        }

        // POST: TrainerServices/Manage/5
        [HttpPost]
        public async Task<IActionResult> Manage(int id, List<TrainerServiceCheckboxViewModel> model)
        {
            var trainer = await _context.Trainers.FindAsync(id);
            if (trainer == null)
                return NotFound();

            // Eski kayıtları sil
            var old = _context.TrainerServices.Where(x => x.TrainerId == id);
            _context.TrainerServices.RemoveRange(old);

            // Yeni seçilenleri ekle
            foreach (var item in model.Where(x => x.IsSelected))
            {
                _context.TrainerServices.Add(new TrainerService
                {
                    TrainerId = id,
                    GymServiceId = item.ServiceId
                });
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Trainers");
        }
    }

    public class TrainerServiceCheckboxViewModel
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = "";
        public bool IsSelected { get; set; }
    }
}
