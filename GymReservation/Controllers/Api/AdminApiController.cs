using System;
using System.Linq;
using System.Threading.Tasks;
using GymReservation.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymReservation.Models;

namespace GymReservation.Controllers.Api
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminApiController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //  Üyeler (arama ile)
 
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers([FromQuery] string? search)
        {
            var q = _userManager.Users.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                q = q.Where(u =>
                    (u.FirstName != null && u.FirstName.Contains(search)) ||
                    (u.LastName != null && u.LastName.Contains(search)) ||
                    (u.Email != null && u.Email.Contains(search)));
            }

            var list = await q
                .OrderBy(u => u.FirstName).ThenBy(u => u.LastName)
                .Select(u => new
                {
                    u.Id,
                    u.FirstName,
                    u.LastName,
                    u.Email
                })
                .ToListAsync();

            return Ok(list);
        }

        // Salonlar (dropdown için)
       
        [HttpGet("fitnesscenters")]
        public async Task<IActionResult> GetFitnessCenters()
        {
            var list = await _context.FitnessCenters
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .Select(x => new { x.Id, x.Name })
                .ToListAsync();

            return Ok(list);
        }

        // Antrenörler (salon filtresi ile)
 
        [HttpGet("trainers")]
        public async Task<IActionResult> GetTrainers([FromQuery] int? fitnessCenterId)
        {
            var q = _context.Trainers
                .AsNoTracking()
                .Include(t => t.FitnessCenter)
                .AsQueryable();

            if (fitnessCenterId.HasValue)
                q = q.Where(t => t.FitnessCenterId == fitnessCenterId.Value);

            var list = await q
                .OrderBy(t => t.FullName)
                .Select(t => new
                {
                    t.Id,
                    t.FullName,
                    t.Specialty,
                    FitnessCenterName = t.FitnessCenter != null ? t.FitnessCenter.Name : ""
                })
                .ToListAsync();

            return Ok(list);
        }

        //  Randevular (durum + tarih aralığı filtresi)
     
        [HttpGet("appointments")]
        public async Task<IActionResult> GetAppointments(
            [FromQuery] string? status,
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to)
        {
            var q = _context.Appointments
                .AsNoTracking()
                .Include(a => a.User)
                .Include(a => a.Trainer)
                    .ThenInclude(t => t.FitnessCenter)
                .Include(a => a.GymService)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
                q = q.Where(a => a.Status == status);

            if (from.HasValue)
                q = q.Where(a => a.StartDateTime >= from.Value.Date);

            if (to.HasValue)
                q = q.Where(a => a.StartDateTime < to.Value.Date.AddDays(1));

            var list = await q
                .OrderByDescending(a => a.StartDateTime)
                .Select(a => new
                {
                    a.Id,
                    a.StartDateTime,
                    a.DurationMinutes,
                    a.Price,
                    a.Status,
                    UserName = (a.User != null ? (a.User.FirstName + " " + a.User.LastName) : ""),
                    TrainerName = (a.Trainer != null ? a.Trainer.FullName : ""),
                    ServiceName = (a.GymService != null ? a.GymService.Name : ""),
                    FitnessCenterName = (a.Trainer != null && a.Trainer.FitnessCenter != null ? a.Trainer.FitnessCenter.Name : "")
                })
                .ToListAsync();

            return Ok(list);
        }
    }
}
