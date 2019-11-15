using System;

namespace Core
{
    public class InvalidHistoryException : Exception
    {
        public InvalidHistoryException(string message)
            :base(message)
        {
            
        }
    }
}