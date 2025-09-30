using AutoMapper;
using TopicDetail.Domain.Models;  // Cho Answer  
using TopicDetail.Application.DTOs;  // Cho AnswerDto, CreateAnswerDto, UpdateAnswerDto  

namespace TopicDetail.Application.Profiles
{
    public class TopicDetailMapping : Profile
    {
        public TopicDetailMapping()
        {
            CreateMap<Answer, AnswerDto>();
            CreateMap<CreateAnswerDto, Answer>();
            CreateMap<UpdateAnswerDto, Answer>();
        }
    }
}
