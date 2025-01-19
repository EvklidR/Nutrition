using AutoMapper;
using PostService.BusinessLogic.DTOs.Comment;
using PostService.Core.Entities;

namespace PostService.BusinessLogic.Mappers
{
    public class CommentMappingProfile : Profile
    {
        public CommentMappingProfile() 
        {
            CreateMap<CreateCommentDTO, Comment>()
                .ForMember(
                dest => dest.Date,
                opt => opt.MapFrom((src, dest) => dest.Date = DateOnly.FromDateTime(DateTime.Now)));

            CreateMap<UpdateCommentDTO, Comment>();
        }
    }
}
