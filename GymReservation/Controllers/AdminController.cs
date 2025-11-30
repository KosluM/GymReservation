using System;
using System.Linq;
using System.Threading.Tasks;
using GymReservation.Data;
using GymReservation.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymReservation.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // /Admin/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var today = DateTime.Today;

            var totalUsers = await _userManager.Users.CountAsync();
            var totalTrainers = await _context.Trainers.CountAsync();
            var totalAppointments = await _context.Appointments.CountAsync();
            var pendingAppointments = await _context.Appointments
                .CountAsync(a => a.Status == "Beklemede");

            var todaysAppointments = await _context.Appointments
                .Include(a => a.Trainer)
                .Include(a => a.User)
                .Include(a => a.GymService)
                .Where(a => a.StartDateTime.Date == today)
                .OrderBy(a => a.StartDateTime)
                .ToListAsync();

            var model = new AdminDashboardViewModel
            {
                Today = today,
                TotalUsers = totalUsers,
                TotalTrainers = totalTrainers,
                TotalAppointments = totalAppointments,
                PendingAppointments = pendingAppointments,
                TodayAppointmentsCount = todaysAppointments.Count,
                TodaysAppointments = todaysAppointments
            };

            return View(model);
        }
    }
}
