using AbpWebProject.Application.Contracts.Jwt.Dto;
using AbpWebProject.Application.Contracts.Users.Dtos;
using AbpWebProject.Domain.Domains;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.AutoMapper;
using Volo.Abp.Users;

namespace AbpWebProject.Application
{
    public class AbpWebProjectApplicationAutoMapProfile : Profile
    {
        public AbpWebProjectApplicationAutoMapProfile()
        {
            // 用户
            CreateMap<User, JwtUserDto>();
            CreateMap<CreateUserDto, User>(MemberList.Source)
               .ForSourceMember(x => x.Password, opt => opt.DoNotValidate())
               ;
            CreateMap<UpdateUserDto, User>(MemberList.Source);
            CreateMap<User, UserDto>(MemberList.Destination);

            // 用户转CurrentUserDto
            CreateMap<User, JwtUserDto>(MemberList.Destination);
            CreateMap<ICurrentUser, JwtUserDto>(MemberList.Destination);
        }
    }
}
