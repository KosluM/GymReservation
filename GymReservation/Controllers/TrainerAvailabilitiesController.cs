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
    [Authorize(Roles = "Admin")]
    public class TrainerAvailabilitiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TrainerAvailabilitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TrainerAvailabilities
        public async Task<IActionResult> Index()
        {
            var list = await _context.TrainerAvailabilities
                .Include(t => t.Trainer)
                .OrderBy(t => t.Date)
                .ThenBy(t => t.StartTime)
                .ToListAsync();

            return View(list);
        }

        // GET: TrainerAvailabilities/Create
        public IActionResult Create()
        {
            ViewData["TrainerId"] = new SelectList(_context.Trainers, "Id", "FullName");
            return View();
        }

        // POST: TrainerAvailabilities/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TrainerAvailability trainerAvailability)
        {
            if (trainerAvailability.EndTime <= trainerAvailability.StartTime)
            {
                ModelState.AddModelError("", "Bitiş saati, başlangıç saatinden büyük olmalıdır.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(trainerAvailability);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TrainerId"] = new SelectList(_context.Trainers, "Id", "FullName", trainerAvailability.TrainerId);
            return View(trainerAvailability);
        }

        // GET: TrainerAvailabilities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var trainerAvailability = await _context.TrainerAvailabilities.FindAsync(id);
            if (trainerAvailability == null) return NotFound();

            ViewData["TrainerId"] = new SelectList(_context.Trainers, "Id", "FullName", trainerAvailability.TrainerId);
            return View(trainerAvailability);
        }

        // POST: TrainerAvailabilities/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TrainerAvailability trainerAvailability)
        {
            if (id != trainerAvailability.Id) return NotFound();

            if (trainerAvailability.EndTime <= trainerAvailability.StartTime)
            {
                ModelState.AddModelError("", "Bitiş saati, başlangıç saatinden büyük olmalıdır.");
            }

            if (ModelState.IsValid)
            {
                _context.Update(trainerAvailability);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["TrainerId"] = new SelectList(_context.Trainers, "Id", "FullName", trainerAvailability.TrainerId);
            return View(trainerAvailability);
        }

        // GET: TrainerAvailabilities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var trainerAvailability = await _context.TrainerAvailabilities
                .Include(t => t.Trainer)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (trainerAvailability == null) return NotFound();

            return View(trainerAvailability);
        }

        // POST: TrainerAvailabilities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trainerAvailability = await _context.TrainerAvailabilities.FindAsync(id);
            if (trainerAvailability != null)
            {
                _context.TrainerAvailabilities.Remove(trainerAvailability);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
