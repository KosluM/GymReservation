using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GymReservation.Data;
using GymReservation.Models;
using Microsoft.AspNetCore.Authorization;

namespace GymReservation.Controllers
{
    [Authorize] // herkes giriş yapınca görebilir
    public class TrainerAvailabilitiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TrainerAvailabilitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // LISTE - Herkes görebilir
        public async Task<IActionResult> Index()
        {
            var list = await _context.TrainerAvailabilities
                .Include(t => t.Trainer)
                .OrderBy(t => t.Date)
                .ThenBy(t => t.StartTime)
                .ToListAsync();

            return View(list);
        }

        // --- SADECE ADMIN ---
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["TrainerId"] = new SelectList(_context.Trainers, "Id", "FullName");
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TrainerAvailability trainerAvailability)
        {
            if (trainerAvailability.EndTime <= trainerAvailability.StartTime)
                ModelState.AddModelError("", "Bitiş saati başlangıçtan büyük olmalı.");

            if (ModelState.IsValid)
            {
                _context.Add(trainerAvailability);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["TrainerId"] = new SelectList(_context.Trainers, "Id", "FullName");
            return View(trainerAvailability);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var entity = await _context.TrainerAvailabilities.FindAsync(id);
            if (entity == null) return NotFound();

            ViewData["TrainerId"] = new SelectList(_context.Trainers, "Id", "FullName", entity.TrainerId);
            return View(entity);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TrainerAvailability trainerAvailability)
        {
            if (id != trainerAvailability.Id) return NotFound();

            if (trainerAvailability.EndTime <= trainerAvailability.StartTime)
                ModelState.AddModelError("", "Bitiş saati başlangıçtan büyük olmalı.");

            if (ModelState.IsValid)
            {
                _context.Update(trainerAvailability);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["TrainerId"] = new SelectList(_context.Trainers, "Id", "FullName", trainerAvailability.TrainerId);
            return View(trainerAvailability);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var entity = await _context.TrainerAvailabilities
                .Include(t => t.Trainer)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (entity == null) return NotFound();

            return View(entity);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var entity = await _context.TrainerAvailabilities.FindAsync(id);

            if (entity != null)
                _context.TrainerAvailabilities.Remove(entity);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
