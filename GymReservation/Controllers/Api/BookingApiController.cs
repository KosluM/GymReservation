using System;
using System.Linq;
using System.Threading.Tasks;
using GymReservation.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymReservation.Controllers.Api
{
    [Route("api/booking")]
    [ApiController]
    [Authorize]
    public class BookingApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BookingApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /api/booking/services?fitnessCenterId=1
        [HttpGet("services")]
        public async Task<IActionResult> GetServices([FromQuery] int fitnessCenterId)
        {
            if (fitnessCenterId <= 0) return BadRequest("fitnessCenterId zorunludur.");

            var services = await _context.GymServices
                .AsNoTracking()
                .Where(s => s.FitnessCenterId == fitnessCenterId)
                .OrderBy(s => s.Name)
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.DurationMinutes,
                    s.Price
                })
                .ToListAsync();

            return Ok(services);
        }

        // GET: /api/booking/trainers?fitnessCenterId=1&gymServiceId=2
        [HttpGet("trainers")]
        public async Task<IActionResult> GetTrainers([FromQuery] int fitnessCenterId, [FromQuery] int gymServiceId)
        {
            if (fitnessCenterId <= 0 || gymServiceId <= 0)
                return BadRequest("fitnessCenterId ve gymServiceId zorunludur.");

            var trainers = await _context.Trainers
                .AsNoTracking()
                .Where(t => t.FitnessCenterId == fitnessCenterId)
                .Where(t => t.TrainerServices.Any(ts => ts.GymServiceId == gymServiceId))
                .OrderBy(t => t.FullName)
                .Select(t => new
                {
                    t.Id,
                    t.FullName,
                    t.Specialty
                })
                .ToListAsync();

            return Ok(trainers);
        }

        // GET: /api/booking/availabilities?trainerId=5&date=2025-12-16
        [HttpGet("availabilities")]
        public async Task<IActionResult> GetAvailabilities([FromQuery] int trainerId, [FromQuery] DateTime date)
        {
            if (trainerId <= 0) return BadRequest("trainerId zorunludur.");

            var list = await _context.TrainerAvailabilities
                .AsNoTracking()
                .Where(a => a.TrainerId == trainerId && a.Date.Date == date.Date)
                .OrderBy(a => a.StartTime)
                .Select(a => new
                {
                    a.Id,
                    Date = a.Date,
                    StartTime = a.StartTime.ToString(@"hh\:mm"),
                    EndTime = a.EndTime.ToString(@"hh\:mm")
                })
                .ToListAsync();

            return Ok(list);
        }
    }
}
