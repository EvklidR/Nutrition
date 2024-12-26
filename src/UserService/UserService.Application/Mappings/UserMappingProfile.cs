using UserService.Application.DTOs;
using UserService.Domain.Entities;


namespace UserService.Application.Mappings
{
    public class UserProfileMapper : AutoMapper.Profile
    {
        public UserProfileMapper()
        {
            CreateMap<CreateUserDTO, User>()
                .ForMember(dest => dest.HashedPassword, opt => opt.MapFrom(src => HashPassword(src.Password)))
                .ForMember(dest => dest.Role, opt => opt.Ignore());
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }

}