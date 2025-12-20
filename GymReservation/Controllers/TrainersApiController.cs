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

    
        //  Örnek 1: Tüm antrenörleri listeleme (REST API)
        [HttpGet]
        public async Task<IActionResult> GetAllTrainers()
        {
            var trainers = await _context.Trainers
                .AsNoTracking()
                .OrderBy(t => t.FullName)
                .Select(t => new
                {
                    t.Id,
                    t.FullName,
                    t.Specialty,
                    t.FitnessCenterId
                })
                .ToListAsync();

            return Ok(trainers);
        }

       
        // Örnek 2: Belirli bir tarihte + hizmete göre uygun antrenörleri getirme
        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableTrainers(DateTime date, int? serviceId, int? fitnessCenterId)
        {
            if (date == default)
                return BadRequest("date parametresi zorunludur. Format: yyyy-MM-dd");

            var targetDate = date.Date;

            var query = _context.Trainers
                .AsNoTracking()
                .Include(t => t.Availabilities)
                .Include(t => t.TrainerServices)
                .AsQueryable();

            // (Opsiyonel) Hizmete göre filtre
            if (serviceId.HasValue)
            {
                query = query.Where(t => t.TrainerServices.Any(ts => ts.GymServiceId == serviceId.Value));
            }

            // (Opsiyonel) Salona göre filtre
            if (fitnessCenterId.HasValue)
            {
                query = query.Where(t => t.FitnessCenterId == fitnessCenterId.Value);
            }

            //  Belirli tarihte availability var mı? 
            var availableTrainers = await query
                .Where(t => t.Availabilities.Any(a => a.Date.Date == targetDate))
                .Select(t => new
                {
                    t.Id,
                    t.FullName,
                    t.Specialty,
                    t.FitnessCenterId
                })
                .ToListAsync();

            return Ok(availableTrainers);
        }
    }
}
