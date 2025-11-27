using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymReservation.Data;
using GymReservation.Models;
using Microsoft.AspNetCore.Authorization;

namespace GymReservation.Controllers
{
    public class TrainersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TrainersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // H everyone: Antrenör listesi (kartlı göstereceğiz)
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var trainers = await _context.Trainers.ToListAsync();
            return View(trainers);
        }

        // Sadece admin görsün diye Details'i de korumak istersen buraya [AllowAnonymous] ekleme
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var trainer = await _context.Trainers
                .FirstOrDefaultAsync(m => m.Id == id);

            if (trainer == null) return NotFound();

            return View(trainer);
        }

        // GET: Trainers/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Trainers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Trainer trainer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(trainer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(trainer);
        }

        // GET: Trainers/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var trainer = await _context.Trainers.FindAsync(id);
            if (trainer == null) return NotFound();

            return View(trainer);
        }

        // POST: Trainers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Trainer trainer)
        {
            if (id != trainer.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(trainer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(trainer);
        }

        // GET: Trainers/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var trainer = await _context.Trainers
                .FirstOrDefaultAsync(m => m.Id == id);

            if (trainer == null) return NotFound();

            return View(trainer);
        }

        // POST: Trainers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trainer = await _context.Trainers.FindAsync(id);
            if (trainer != null)
            {
                _context.Trainers.Remove(trainer);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
