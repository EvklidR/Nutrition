using AutoMapper;
using PostService.BusinessLogic.DTOs.Requests.Post;
using PostService.BusinessLogic.DTOs.Responses.Post;
using PostService.Core.Entities;

namespace PostService.BusinessLogic.Mappers
{
    public class PostMappingProfile : Profile
    {
        public PostMappingProfile() 
        {
            CreateMap<CreatePostDTO, Post>()
                .ForMember(
                dest => dest.CreationDate, 
                opt => opt.MapFrom((src, dest) => dest.CreationDate = DateTime.Now));

            CreateMap<UpdatePostDTO, Post>();

            CreateMap<Post, PostResponse>()
                .ForMember(dest => dest.AmountOfComments, opt => opt.MapFrom(src => src.Comments.Count))
                .ForMember(dest => dest.AmountOfLikes, opt => opt.MapFrom(src => src.UserLikeIds.Count))
                .ForMember(dest => dest.IsLiked, opt => opt.MapFrom((src, dest, destMember, context) =>
                    src.UserLikeIds.Contains(context.Items["CurrentUserId"].ToString())))
                .ForMember(dest => dest.IsOwner, opt => opt.MapFrom((src, dest, destMember, context) =>
                    src.OwnerId == context.Items["CurrentUserId"].ToString()));
        }
    }
}
