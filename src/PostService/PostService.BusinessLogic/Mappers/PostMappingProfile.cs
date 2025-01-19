using AutoMapper;
using PostService.BusinessLogic.DTOs.Post;
using PostService.Core.Entities;

namespace PostService.BusinessLogic.Mappers
{
    public class PostMappingProfile : Profile
    {
        public PostMappingProfile() 
        {
            CreateMap<CreatePostDTO, Post>()
                .ForMember(
                dest => dest.Date, 
                opt => opt.MapFrom((src, dest) => dest.Date = DateOnly.FromDateTime(DateTime.Now)));

            CreateMap<UpdatePostDTO, Post>();

            CreateMap<Post, PostDTO>()
                .ForMember(dest => dest.AmountOfComments, opt => opt.MapFrom(src => src.Comments.Count))
                .ForMember(dest => dest.AmountOfLikes, opt => opt.MapFrom(src => src.UserLikeIds.Count))
                .ForMember(dest => dest.IsLiked, opt => opt.MapFrom((src, dest, destMember, context) =>
                    src.UserLikeIds.Contains(context.Items["CurrentUserId"].ToString())));
        }
    }
}
