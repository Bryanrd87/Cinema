using System;

namespace Application.Exceptions
{
    public class ReservationAlreadyPaidException : Exception
    {
        public ReservationAlreadyPaidException(object key) : base($"Reservation ({key}) has already been paid.")
        {
        }
    }
}
