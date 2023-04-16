using System;

namespace iRacingPDFLoader.Exceptions
{
    public class ScheduleLoaderException : Exception
    {
        internal static readonly string MSG_NOTFOUND_PDF = "No existe el PDF: {0}";
        public ScheduleLoaderException() { }
        public ScheduleLoaderException(string message)
        : base(message) { }
    }
}
