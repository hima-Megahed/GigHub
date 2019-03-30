using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using GigHub.Core.Dtos;
using GigHub.Core.Models;

namespace GigHub.App_Start
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ApplicationUser, UserDto>();
            CreateMap<Gig, GigDto>();
            CreateMap<Notification, NotificationDto>();
        }
    }
}