using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Homework.Domain.DTOs;

namespace Homework.Application.Profiles
{
    public class HomeworkProfile : Profile
    {
        public HomeworkProfile()
        {
            // Entity -> DTO
            CreateMap<Homework.Domain.Entities.HomeWork, HomeworkDto>()
                .ForMember(dest => dest.HomeworkID, opt => opt.MapFrom(src => src.HomeworkId))
                .ForMember(dest => dest.TopicID, opt => opt.MapFrom(src => src.TopicId));

            // DTO -> Entity
            CreateMap<HomeworkDto, Homework.Domain.Entities.HomeWork>()
                .ForMember(dest => dest.HomeworkId, opt => opt.MapFrom(src => src.HomeworkID))
                .ForMember(dest => dest.TopicId, opt => opt.MapFrom(src => src.TopicID));

            // Nếu có HomeworkCreateDto thì thêm luôn
            CreateMap<HomeworkCreateDto, Homework.Domain.Entities.HomeWork>();
        }
    }
}
