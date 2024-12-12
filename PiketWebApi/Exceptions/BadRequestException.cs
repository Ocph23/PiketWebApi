namespace PiketWebApi.Exceptions
{
    public class BadRequestException : Exception
    {
        public BadRequestException()
        {

        }

        public BadRequestException(string message) : base(message)
        {

        }

        public BadRequestException(string message, Dictionary<string, object> errors):base(message)
        {
            Errors = errors;
        }

        public Dictionary<string,object> Errors { get; set; }

    }
}
