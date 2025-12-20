using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymReservation.Data;
using GymReservation.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymReservation.Controllers
{
    [Authorize] // login olan herkes görebilir
    public class TrainersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TrainersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Salon dropdown
        private void PopulateFitnessCenters(int? selectedId = null)
        {
            ViewBag.FitnessCenterId = new SelectList(
                _context.FitnessCenters.OrderBy(x => x.Name),
                "Id",
                "Name",
                selectedId
            );
        }

        // Checkbox'lar için hizmet listesini hazırlar
        private void PopulateServicesForCheckboxes(IEnumerable<int>? selectedIds = null)
        {
            var services = _context.GymServices
                .OrderBy(s => s.Name)
                .ToList();

            ViewBag.Services = services;
            ViewBag.SelectedServiceIds = selectedIds?.ToList() ?? new List<int>();
        }

        // Liste - Herkes görebilir
        public async Task<IActionResult> Index()
        {
            var trainers = await _context.Trainers
                .Include(t => t.FitnessCenter)
                .Include(t => t.TrainerServices)
                .ToListAsync();

            return View(trainers);
        }

        // Detay - Herkes görebilir
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var trainer = await _context.Trainers
                .Include(t => t.FitnessCenter)
                .Include(t => t.TrainerServices)
                .ThenInclude(ts => ts.GymService)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (trainer == null) return NotFound();

            return View(trainer);
        }

        // --- SADECE ADMIN ---

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            PopulateFitnessCenters();
            PopulateServicesForCheckboxes();
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Trainer trainer, int[] selectedServiceIds)
        {
            if (!ModelState.IsValid)
            {
                PopulateFitnessCenters(trainer.FitnessCenterId);
                PopulateServicesForCheckboxes(selectedServiceIds);
                return View(trainer);
            }

            _context.Add(trainer);
            await _context.SaveChangesAsync();

            if (selectedServiceIds != null && selectedServiceIds.Length > 0)
            {
                foreach (var sid in selectedServiceIds)
                {
                    _context.TrainerServices.Add(new TrainerService
                    {
                        TrainerId = trainer.Id,
                        GymServiceId = sid
                    });
                }
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var trainer = await _context.Trainers
                .Include(t => t.TrainerServices)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (trainer == null) return NotFound();

            PopulateFitnessCenters(trainer.FitnessCenterId);

            var selectedIds = trainer.TrainerServices?
                .Select(ts => ts.GymServiceId)
                .ToList();

            PopulateServicesForCheckboxes(selectedIds);

            return View(trainer);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Trainer trainer, int[] selectedServiceIds)
        {
            if (id != trainer.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                PopulateFitnessCenters(trainer.FitnessCenterId);
                PopulateServicesForCheckboxes(selectedServiceIds);
                return View(trainer);
            }

            _context.Update(trainer);
            await _context.SaveChangesAsync();

            // Eski eşleşmeleri sil
            var old = _context.TrainerServices.Where(ts => ts.TrainerId == trainer.Id);
            _context.TrainerServices.RemoveRange(old);

            // Yeni seçilenleri ekle
            if (selectedServiceIds != null && selectedServiceIds.Length > 0)
            {
                foreach (var sid in selectedServiceIds)
                {
                    _context.TrainerServices.Add(new TrainerService
                    {
                        TrainerId = trainer.Id,
                        GymServiceId = sid
                    });
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var trainer = await _context.Trainers
                .Include(t => t.FitnessCenter)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (trainer == null) return NotFound();

            return View(trainer);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trainer = await _context.Trainers.FindAsync(id);

            if (trainer != null)
                _context.Trainers.Remove(trainer);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
