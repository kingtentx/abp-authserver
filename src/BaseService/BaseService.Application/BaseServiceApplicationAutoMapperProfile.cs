using AutoMapper;

using BaseService.BaseData;
using BaseService.BaseData.DataDictionaryManagement.Dto;
using BaseService.BaseData.OrganizationManagement.Dto;
using BaseService.BaseData.PositionManagement.Dto;
using BaseService.Systems;
using BaseService.Systems.AuditLoggingManagement.Dto;
using BaseService.Systems.AuthorityManagerment.Dto;
using BaseService.Systems.EdgeConfigManagement.Dto;
using BaseService.Systems.MenuManagement.Dto;
using BaseService.Systems.RoleManagement.Dto;
using BaseService.Systems.RoleMenusManagement.Dto;
using BaseService.Systems.UserManagement.Dto;
using Volo.Abp.AuditLogging;
using Volo.Abp.Identity;
using Volo.Abp.TenantManagement;
using Volo.Abp.Users;

namespace BaseService
{
    public class BaseServiceApplicationAutoMapperProfile : Profile
    {
        public BaseServiceApplicationAutoMapperProfile()
        {
            CreateMap<Tenant, TenantDto>().ReverseMap();

            CreateMap<IdentityUser, BaseIdentityUserDto>().ReverseMap();
            CreateMap<User, BaseIdentityUserDto>().ReverseMap();
            CreateMap<BaseIdentityUserCreateDto, BaseIdentityUserDto>().ReverseMap();
            CreateMap<CurrentUser, CurrentUserDto>().ReverseMap();

            CreateMap<IdentityRole, BaseIdentityRoleDto>().ReverseMap();
            CreateMap<Role, BaseIdentityRoleDto>().ReverseMap();

            CreateMap<AuditLog, AuditLogDto>()
                .ForMember(t => t.EntityChanges, opt => opt.MapFrom(p => p.EntityChanges))
                .ForMember(t => t.Actions, opt => opt.MapFrom(p => p.Actions)).ReverseMap();

            CreateMap<EntityChange, EntityChangeDto>()
                 .ForMember(t => t.PropertyChanges, option => option.MapFrom(p => p.PropertyChanges)).ReverseMap();

            CreateMap<AuditLogAction, AuditLogActionDto>().ReverseMap();
            CreateMap<EntityPropertyChange, EntityPropertyChangeDto>().ReverseMap();

            CreateMap<DataDictionary, DictionaryDto>().ReverseMap();
            CreateMap<DataDictionaryDetail, DictionaryDetailDto>().ReverseMap();

            CreateMap<Organization, OrganizationDto>()
                .ForMember(dto => dto.Label, opt => opt.MapFrom(p => p.Name)).ReverseMap();

            CreateMap<Position, PositionDto>().ReverseMap();

            CreateMap<Menu, MenuDto>().ReverseMap();
            CreateMap<Menu, MenusTreeDto>().ReverseMap();

            CreateMap<Authority, AuthorityDto>().ReverseMap();
            CreateMap<AuthorityDetail, AuthorityDetailDto>().ReverseMap();
            CreateMap<AuthorityGroup, AuthorityGroupDto>().ReverseMap();

            CreateMap<EdgeConfig, EdgeConfigDto>().ReverseMap();


        }
    }
}
