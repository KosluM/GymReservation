using System;
using System.Linq;
using System.Threading.Tasks;
using GymReservation.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymReservation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrainersApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TrainersApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/TrainersApi
        [HttpGet]
        public async Task<IActionResult> GetAllTrainers()
        {
            var trainers = await _context.Trainers
                .Select(t => new
                {
                    t.Id,
                    t.FullName,
                    Specialty = t.Specialty   // <- burası düzeltildi
                })
                .ToListAsync();

            return Ok(trainers);
        }

        // GET: api/TrainersApi/available?date=2025-11-28&serviceId=1
        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableTrainers(DateTime date, int? serviceId)
        {
            if (date == default)
                return BadRequest("date parametresi zorunludur. Format: yyyy-MM-dd");

            var dayOfWeek = date.DayOfWeek;

            var query = _context.Trainers
                .Include(t => t.Availabilities)
                .Include(t => t.TrainerServices)
                    .ThenInclude(ts => ts.GymService)
                .AsQueryable();

            if (serviceId.HasValue)
            {
                query = query.Where(t => t.TrainerServices.Any(ts => ts.GymServiceId == serviceId.Value));
            }

            var availableTrainers = await query
                .Where(t => t.Availabilities.Any(a => a.Date.DayOfWeek == dayOfWeek))  // <- burası düzeltildi
                .Select(t => new
                {
                    t.Id,
                    t.FullName,
                    Specialty = t.Specialty
                })
                .ToListAsync();

            return Ok(availableTrainers);
        }
    }
}
