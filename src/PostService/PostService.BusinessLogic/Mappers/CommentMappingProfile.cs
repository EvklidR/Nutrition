using AutoMapper;
using PostService.BusinessLogic.DTOs.Requests.Comment;
using PostService.BusinessLogic.DTOs.Responses.Comment;
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

            CreateMap<Comment, CommentResponse>()
                .ForMember(dest => dest.AmountOfLikes, opt => opt.MapFrom(src => src.UserLikeIds.Count))
                .ForMember(dest => dest.IsOwner, opt => opt.MapFrom((src, dest, destMember, context) =>
                    src.OwnerId == context.Items["CurrentUserId"].ToString()))
                .ForMember(dest => dest.IsLiked, opt => opt.MapFrom((src, dest, destMember, context) =>
                    src.UserLikeIds.Contains(context.Items["CurrentUserId"].ToString()!)));
        }
    }
}
