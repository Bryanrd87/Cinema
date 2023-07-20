using System.ComponentModel;

namespace Application.Models.Identity
{
    public class AuthRequest
    {
        [DefaultValue("user@example.com")]
        public string Username { get; set; }
        [DefaultValue("MyPass123")]
        public string Password { get; set; }
        [DefaultValue("Admin")]
        public string Role { get; set; }
    }
}
