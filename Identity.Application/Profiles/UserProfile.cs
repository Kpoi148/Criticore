using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Identity.Domain.DTOs;
using AutoMapper;
using Identity.Domain.Entities;

namespace Identity.Application.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            // Entity -> DTO
            CreateMap<User, UserDto>();

            // DTO -> Entity
            CreateMap<UserCreateDto, User>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(_ => string.Empty)) // external auth default
                .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => "Active"));

            CreateMap<UserUpdateDto, User>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()) // không cho update password ở đây
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()); // không update CreatedAt
        }
    }
}
