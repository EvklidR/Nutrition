namespace PostService.BusinessLogic.Exceptions
{
    public class Forbidden : Exception
    {
        public Forbidden(string message) : base(message) { }
    }
}
