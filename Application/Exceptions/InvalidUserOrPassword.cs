using System;

namespace Application.Exceptions
{
    public class InvalidUserOrPassword : Exception
    {
        public InvalidUserOrPassword(string user) : base($"Invalid user: {user} or password")
        {
        }
    }
}
