using Microsoft.AspNetCore.Identity;

namespace GymReservation.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";

        // Read-only (sadece get), buna değer atayamayız
        public string FullName => $"{FirstName} {LastName}".Trim();
    }
}
