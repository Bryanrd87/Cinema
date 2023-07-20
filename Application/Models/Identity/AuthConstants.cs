using System.Collections.Generic;

namespace Application.Models.Identity
{
    public class AuthConstants
    {
        public static List<AuthResponse> Users = new List<AuthResponse>()
        {
            new AuthResponse(){ Username="user@example.com",Password="MyPass123",Role="Admin"}
        };
    }
}
