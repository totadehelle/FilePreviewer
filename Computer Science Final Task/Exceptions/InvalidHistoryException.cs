using System;

namespace Computer_Science_Final_Task.Exceptions
{
    public class InvalidHistoryException : Exception
    {
        public InvalidHistoryException(string message)
            :base(message)
        {
            
        }
    }
}