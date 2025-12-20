using System.Linq;
using System.Threading.Tasks;
using GymReservation.Data;
using GymReservation.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

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

        // MÜŞTERİ + ADMIN: Liste
        public async Task<IActionResult> Index()
        {
            var list = await _context.GymServices
                .Include(s => s.FitnessCenter)
                .ToListAsync();

            return View(list);
        }

        // MÜŞTERİ + ADMIN: Detay
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var service = await _context.GymServices
                .Include(s => s.FitnessCenter)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (service == null)
                return NotFound();

            return View(service);
        }

        // ---- yardımcı: salon dropdown'ı doldur ----
        private void PopulateFitnessCentersDropDown(int? selectedId = null)
        {
            var centers = _context.FitnessCenters
                .OrderBy(c => c.Name)
                .ToList();

            ViewBag.FitnessCenters = new SelectList(centers, "Id", "Name", selectedId);
        }

        // SADECE ADMIN: Create GET
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            PopulateFitnessCentersDropDown();
            return View();
        }

        // SADECE ADMIN: Create POST
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GymService gymService)
        {
            if (ModelState.IsValid)
            {
                _context.Add(gymService);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            PopulateFitnessCentersDropDown(gymService.FitnessCenterId);
            return View(gymService);
        }

        // SADECE ADMIN: Edit GET
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var service = await _context.GymServices.FindAsync(id);
            if (service == null)
                return NotFound();

            PopulateFitnessCentersDropDown(service.FitnessCenterId);
            return View(service);
        }

        // SADECE ADMIN: Edit POST
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, GymService gymService)
        {
            if (id != gymService.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                PopulateFitnessCentersDropDown(gymService.FitnessCenterId);
                return View(gymService);
            }

            try
            {
                _context.Update(gymService);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                bool exists = await _context.GymServices.AnyAsync(x => x.Id == gymService.Id);
                if (!exists) return NotFound();
                throw;
            }
        }


        // SADECE ADMIN: Delete GET
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var service = await _context.GymServices
                .Include(s => s.FitnessCenter)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (service == null)
                return NotFound();

            return View(service);
        }

        // SADECE ADMIN: Delete POST
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var service = await _context.GymServices.FindAsync(id);
            if (service != null)
            {
                _context.GymServices.Remove(service);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
