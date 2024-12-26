namespace UserService.Application.Models
{
    public class AuthenticatedResponse
    {
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
    }
}
