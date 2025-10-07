using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Homework.Domain.DTOs;
using AutoMapper;
using Homework.Domain.Entities;

namespace Homework.Application.Profiles
{
    public class SubmissionProfile : Profile
    {
        public SubmissionProfile()
        {
            CreateMap<Submission, SubmissionReadDto>();
            CreateMap<SubmissionCreateDto, Submission>();
            CreateMap<SubmissionUpdateDto, Submission>();
        }
    }
}
