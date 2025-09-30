using AutoMapper;
using TopicDetail.Domain.Models;  // Cho Answer  
using TopicDetail.Application.DTOs;  // Cho AnswerDto, CreateAnswerDto, UpdateAnswerDto  

namespace TopicDetail.Application.Profiles
{
    public class TopicDetailMapping : Profile
    {
        public TopicDetailMapping()
        {
            CreateMap<Answer, AnswerDto>()
    .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.User != null ? src.User.AvatarUrl : "default-url"))
    .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.User != null ? src.User.FullName : "Unknown"));
            CreateMap<CreateAnswerDto, Answer>();
            CreateMap<UpdateAnswerDto, Answer>();
        }
    }
}
