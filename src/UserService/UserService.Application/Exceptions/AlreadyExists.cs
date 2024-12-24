namespace UserService.Application.Exceptions
{
    public class AlreadyExists : Exception
    {
        public AlreadyExists(string message) : base(message) { }
    }
}
