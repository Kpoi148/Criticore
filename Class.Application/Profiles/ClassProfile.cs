using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Class.Domain.DTOs;
using AutoMapper;

namespace Class.Application.Profiles
{
    public class ClassProfile :Profile
    {
        public ClassProfile()
        {

            CreateMap<Class.Domain.Entities.Class, ClassDto>()
                .ForMember(dest => dest.Members, opt => opt.MapFrom(src => src.ClassMembers))
                .ReverseMap();

            CreateMap<Class.Domain.Entities.ClassMember, ClassMemberDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName))
                .ReverseMap();
        }
    }
}
