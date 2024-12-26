namespace UserService.Domain.Entities
{
    public class Confirmation
    {
        public string Email { get; set; } = null!;
        public int Code { get; set; }
    }
}
