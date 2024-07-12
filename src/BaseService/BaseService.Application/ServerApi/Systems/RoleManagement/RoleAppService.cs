using BaseService.Consts;
using BaseService.Systems.RoleManagement.Dto;
using Cimc.Model.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Caching;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;

namespace BaseService.Systems.RoleManagement
{
    /// <summary>
    /// 角色
    /// </summary>
    [Area("Base")]
    [Route("api/base/role")]
    [Authorize]
    public class RoleAppService : BaseApplicationService, IRoleAppService
    {
        /// <summary>
        /// 
        /// </summary>
        protected IdentityRoleManager RoleManager { get; }
        private readonly IRepository<Role, Guid> _roleRepository;
        private readonly IDistributedCache<Role> _cache;
        private readonly IRepository<User, Guid> _userRepository;
        private readonly IRepository<RoleAuthority> _roleAuthorityRepository;
        private readonly IRepository<UserRole> _userRoleRepository;
        private readonly IRepository<Authority> _authorityRepository;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="roleManager"></param>
        /// <param name="roleRepository"></param>
        /// <param name="cache"></param>
        public RoleAppService(
             IdentityRoleManager roleManager,
             IRepository<Role, Guid> roleRepository,
             IDistributedCache<Role> cache,
             IRepository<User, Guid> userRepository,
             IRepository<RoleAuthority> roleAuthorityRepository,
             IRepository<UserRole> userRoleRepository,
             IRepository<Authority> authorityRepository,
             IDefaultAppService defaultAppService
            ) : base(defaultAppService)
        {
            RoleManager = roleManager;
            _roleRepository = roleRepository;
            _cache = cache;
            _userRepository = userRepository;
            _roleAuthorityRepository = roleAuthorityRepository;
            _userRoleRepository = userRoleRepository;
            _authorityRepository = authorityRepository;
        }


        /// <summary>
        /// 创建角色
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("create")]
        [Authorize(IdentityPermissions.Roles.Create)]
        public async Task<ResultDto<Guid>> Create(BaseIdentityRoleCreateDto input)
        {
            var result = new ResultDto<Guid>();

            //var role = new IdentityRole(
            //               GuidGenerator.Create(),
            //               input.Name,
            //               input.TenantId = CurrentTenant.Id
            //           );

            if (input.Name.Trim().Equals(SystemConsts.SuperAdmin))
            {
                result.Message = "系统禁止创建名称为admin的角色";
                return result;
            }
            //var authorityId = CurrentAuthority.Id;
            var authorityId = CurrentAuthority.Id;

            var role = new Role(
                          GuidGenerator.Create(),
                          input.Name,
                          input.Name.ToUpper(),
                          CurrentTenant.Id,
                          authorityId
                      );

            var repeat = await _roleRepository.FirstOrDefaultAsync(p => p.TenantId == role.TenantId && p.Name == role.Name && p.AuthorityId == authorityId);
            if (repeat != null)
            {
                result.Message = "角色名称重复";
                return result;
            }

            role.IsPublic = true; // input.IsPublic;
            role.IsDefault = false; // input.IsDefault;
            //role.ExtraProperties.Add(nameof(AppRole.Sort), input.Sort);
            //role.ExtraProperties.Add(nameof(AppRole.Remark), input.Remark);
            //role.ExtraProperties.Add(nameof(AppRole.IsActive), input.IsActive);
            role.Sort = input.Sort;
            role.Remark = input.Remark;
            role.IsActive = input.IsActive;
            role.AuthorityId = authorityId;

            await _roleRepository.InsertAsync(role);
            await CurrentUnitOfWork.SaveChangesAsync();

            result.SetData(role.Id);
            return result;
        }

        /// <summary>
        /// 更新角色
        /// </summary>
        /// <param name="input"></param>    
        /// <returns></returns>
        [HttpPost]
        [Route("update")]
        [Authorize(IdentityPermissions.Roles.Update)]
        public async Task<ResultDto<bool>> Update(BaseIdentityRoleUpdateDto input)
        {
            var result = new ResultDto<bool>();
            var role = await RoleManager.GetByIdAsync(input.Id);

            if (role.Name != input.Name)
            {
                result.Message = "不允许编辑角色名";
                return result;
            }

            (await RoleManager.SetRoleNameAsync(role, input.Name)).CheckErrors();

            role.IsPublic = true; //input.IsPublic;
            role.IsDefault = false; // input.IsDefault;
            role.SetProperty(nameof(Role.Sort), input.Sort);
            role.SetProperty(nameof(Role.Remark), input.Remark);
            role.SetProperty(nameof(Role.IsActive), input.IsActive);
            //role.SetProperty(nameof(Role.AuthorityId), CurrentAuthority.Id);

            await RoleManager.UpdateAsync(role);
            await CurrentUnitOfWork.SaveChangesAsync();

            result.SetData(true);
            return result;

        }


        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("delete")]
        [Authorize(IdentityPermissions.Roles.Delete)]
        public async Task<ResultDto<bool>> Delete(List<Guid> ids)
        {
            var result = new ResultDto<bool>();

            try
            {
                await _roleRepository.DeleteManyAsync(ids);
                await CurrentUnitOfWork.SaveChangesAsync();

                result.SetData(true);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 查询单实例
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<ResultDto<BaseIdentityRoleDto>> Get(Guid id)
        {
            var result = new ResultDto<BaseIdentityRoleDto>();

            var role = await _roleRepository.FindAsync(id);

            //var role = await _cache.GetOrAddAsync(CacheConsts.UserRoleKey + id.ToString(),
            //    async () => await _roleRepository.FindAsync(id),
            //    () => PolicyHelper.SetPolicy(TimeSpan.FromSeconds(BaseConsts.len_30)));

            var dto = ObjectMapper.Map<Role, BaseIdentityRoleDto>(role);

            //var aids = (await _roleAuthorityRepository.GetQueryableAsync()).Where(p => p.RoleId.Equals(id));
            //var auths = await _authorityRepository.GetQueryableAsync();

            //var query = from aid in aids
            //            join auth in auths on aid.AuthorityId equals auth.Id
            //            select new AuthorityArrary
            //            {
            //                AuthId = auth.Id,
            //                AuthType = auth.AuthType
            //            };

            //dto.AuthorityIds = query.ToList();

            result.SetData(dto);
            return result;
        }

        /// <summary>
        /// 查询分页列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("list")]
        public async Task<ResultDto<PagedResultDto<BaseIdentityRoleDto>>> GetList(GetBaseIdentityRoleInput input)
        {
            var result = new ResultDto<PagedResultDto<BaseIdentityRoleDto>>();

            var query = (await _roleRepository.GetQueryableAsync())
                        .Where(p => p.AuthorityId == CurrentAuthority.Id)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), p => p.Name.Contains(input.Filter));

            //if (CurrentUser.UserName != BaseConsts.SuperAdmin)
            //{
            //    query = query.Where(p => p.AuthorityId == CurrentAuthority.Id);
            //    //query = query.Where(p => CurrentAuthorityIds.Contains(p.AuthorityId.Value));
            //}               


            if (input.IsActive.HasValue)
                query = query.Where(p => p.IsActive == input.IsActive);

            var items = await query.OrderBy(p => p.Sort)
                                   .Skip(input.SkipCount)
                                   .Take(input.MaxResultCount)
                                   .ToListAsync();

            var totalCount = await query.CountAsync();

            var dtos = ObjectMapper.Map<List<Role>, List<BaseIdentityRoleDto>>(items);

            var data = new PagedResultDto<BaseIdentityRoleDto>(totalCount, dtos);

            result.SetData(data);
            return result;
        }

        /// <summary>
        ///  查询所有角色
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("load-roles")]
        public async Task<ResultDto<ListResultDto<BaseIdentityRoleDto>>> LoadAllRoles(string filter)
        {
            var result = new ResultDto<ListResultDto<BaseIdentityRoleDto>>();

            var query = (await _roleRepository.GetQueryableAsync()).WhereIf(!string.IsNullOrWhiteSpace(filter), p => p.Name.Contains(filter));

            if (CurrentUser.UserName != SystemConsts.SuperAdmin)
                query = query.Where(p => p.AuthorityId == CurrentAuthority.Id);

            var items = await query.OrderBy(p => p.Sort).ToListAsync();
            var dto = ObjectMapper.Map<List<Role>, List<BaseIdentityRoleDto>>(items);

            var data = new ListResultDto<BaseIdentityRoleDto>(dto);

            result.SetData(data);
            return result;
        }

        /// <summary>
        /// 用户ID获取角色
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>     
        [HttpGet]
        [Route("roles")]
        public async Task<ResultDto<ListResultDto<BaseIdentityRoleDto>>> GetRoles(Guid userId)
        {
            var result = new ResultDto<ListResultDto<BaseIdentityRoleDto>>();

            var rIds = (await _userRoleRepository.GetListAsync(p => p.UserId == userId)).Select(p => p.RoleId).ToList();

            var role = await _roleRepository.GetListAsync(p => rIds.Contains(p.Id));

            var dtos = ObjectMapper.Map<List<Role>, List<BaseIdentityRoleDto>>(role);
            var data = new ListResultDto<BaseIdentityRoleDto>(dtos);

            result.SetData(data);
            return result;
        }
    }
}
