namespace UserService.Application.DTOs
{
    public class CreateUserDTO
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}