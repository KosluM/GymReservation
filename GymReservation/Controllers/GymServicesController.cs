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
    [Authorize]
    public class GymServicesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GymServicesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // LISTE - Herkes görebilir
        public async Task<IActionResult> Index()
        {
            var list = await _context.GymServices
                .Include(s => s.FitnessCenter)
                .ToListAsync();

            return View(list);
        }

        // --- SADECE ADMIN ---
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewBag.FitnessCenters = new SelectList(_context.FitnessCenters, "Id", "Name");
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GymService gymService)
        {
            if (ModelState.IsValid)
            {
                _context.Add(gymService);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.FitnessCenters = new SelectList(_context.FitnessCenters, "Id", "Name");
            return View(gymService);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var gymService = await _context.GymServices.FindAsync(id);
            if (gymService == null) return NotFound();

            ViewBag.FitnessCenters = new SelectList(_context.FitnessCenters, "Id", "Name", gymService.FitnessCenterId);
            return View(gymService);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, GymService gymService)
        {
            if (id != gymService.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(gymService);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.FitnessCenters = new SelectList(_context.FitnessCenters, "Id", "Name", gymService.FitnessCenterId);
            return View(gymService);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var gymService = await _context.GymServices
                .Include(s => s.FitnessCenter)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (gymService == null) return NotFound();
            return View(gymService);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gymService = await _context.GymServices.FindAsync(id);

            if (gymService != null)
                _context.GymServices.Remove(gymService);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
