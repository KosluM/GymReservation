using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymReservation.Data;
using GymReservation.Models;

namespace GymReservation.Controllers
{
    [Authorize] // sadece giriş yapanlar
    public class FitnessCentersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FitnessCentersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // LISTE - Üye + Admin görebilir
        public async Task<IActionResult> Index()
        {
            var centers = await _context.FitnessCenters.ToListAsync();
            return View(centers);
        }

        // DETAY - Üye + Admin görebilir
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var center = await _context.FitnessCenters
                .FirstOrDefaultAsync(x => x.Id == id);

            if (center == null) return NotFound();

            return View(center);
        }

        // ======== BUNDAN SONRASI SADECE ADMIN ========

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FitnessCenter fitnessCenter)
        {
            if (ModelState.IsValid)
            {
                _context.Add(fitnessCenter);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(fitnessCenter);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var center = await _context.FitnessCenters.FindAsync(id);
            if (center == null) return NotFound();

            return View(center);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FitnessCenter fitnessCenter)
        {
            if (id != fitnessCenter.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(fitnessCenter);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(fitnessCenter);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var center = await _context.FitnessCenters
                .FirstOrDefaultAsync(x => x.Id == id);
            if (center == null) return NotFound();

            return View(center);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var center = await _context.FitnessCenters.FindAsync(id);
            if (center != null)
            {
                _context.FitnessCenters.Remove(center);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
