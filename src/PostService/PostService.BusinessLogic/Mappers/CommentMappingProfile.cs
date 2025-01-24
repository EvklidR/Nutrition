using AutoMapper;
using PostService.BusinessLogic.DTOs.Comment;
using PostService.BusinessLogic.DTOs.Post;
using PostService.Core.Entities;

namespace PostService.BusinessLogic.Mappers
{
    public class CommentMappingProfile : Profile
    {
        public CommentMappingProfile() 
        {
            CreateMap<CreateCommentDTO, Comment>()
                .ForMember(
                dest => dest.CreationDate,
                opt => opt.MapFrom((src, dest) => dest.CreationDate = DateTime.Now));

            CreateMap<UpdateCommentDTO, Comment>();

            CreateMap<Comment, CommentDTO>()
                .ForMember(dest => dest.AmountOfLikes, opt => opt.MapFrom(src => src.UserLikeIds.Count))
                .ForMember(dest => dest.IsLiked, opt => opt.MapFrom((src, dest, destMember, context) =>
                    src.UserLikeIds.Contains(context.Items["CurrentUserId"].ToString())));
        }
    }
}
