namespace FoodService.Application.Exceptions
{
    public class Forbidden : Exception
    {
        public Forbidden(string message) : base(message) { }
    }
}
