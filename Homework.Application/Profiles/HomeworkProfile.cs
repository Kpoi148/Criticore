using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Homework.Domain.DTOs;
using Homework.Domain.Entities;

namespace Homework.Application.Profiles
{
    public class HomeworkProfile : Profile
    {
        public HomeworkProfile()
        {
            // Entity -> DTO
            CreateMap<HomeWork, HomeworkDto>()
                .ForMember(dest => dest.HomeworkID, opt => opt.MapFrom(src => src.HomeworkId))
                .ForMember(dest => dest.TopicID, opt => opt.MapFrom(src => src.TopicId));

            // DTO -> Entity
            CreateMap<HomeworkDto, HomeWork>()
                .ForMember(dest => dest.HomeworkId, opt => opt.MapFrom(src => src.HomeworkID))
                .ForMember(dest => dest.TopicId, opt => opt.MapFrom(src => src.TopicID));

            CreateMap<HomeWork, DeadlineDto>()
                .ForMember(dest => dest.HomeworkId, opt => opt.MapFrom(src => src.HomeworkId))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title ?? string.Empty))
                .ForMember(dest => dest.TopicTitle, opt => opt.MapFrom(src => src.Topic.Title ?? string.Empty))
                .ForMember(dest => dest.ClassName, opt => opt.MapFrom(src => src.Topic.Class.ClassName ?? string.Empty))
                .ForMember(dest => dest.DueDate, opt => opt.MapFrom(src => src.DueDate))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));

            // Nếu có HomeworkCreateDto thì thêm luôn
            CreateMap<HomeworkCreateDto, HomeWork>();
        }
    }
}
