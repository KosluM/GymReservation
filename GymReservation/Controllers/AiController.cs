using System.Threading.Tasks;
using GymReservation.Models;
using GymReservation.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymReservation.Controllers
{
    [Authorize]
    public class AiController : Controller
    {
        private readonly GeminiFitnessService _geminiService;

        public AiController(GeminiFitnessService geminiService)
        {
            _geminiService = geminiService;
        }

        [HttpGet]
        public IActionResult FitnessRecommendation()
        {
            var model = new FitnessAiRequestViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FitnessRecommendation(FitnessAiRequestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _geminiService.GetFitnessPlanAsync(model);
            model.ResultText = result;

            return View(model);
        }
    }
}
