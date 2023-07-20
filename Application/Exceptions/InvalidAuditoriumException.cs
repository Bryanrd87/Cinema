using System;

namespace Application.Exceptions
{
    public class InvalidAuditoriumException : Exception
    {
        public InvalidAuditoriumException(string name,object key) : base($"Invalid {name} id ({key}).")
        {
        }
    }
}
