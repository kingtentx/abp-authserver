
using BaseService.BaseData;
using BaseService.Enums;
using BaseService.Systems.UserManagement.Dto;
using Cimc.Model.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.ObjectExtending;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Users;
using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace BaseService.Systems.UserManagement
{
    /// <summary>
    /// 用户
    /// </summary>
    [Area("Base")]
    [Route("api/base/user")]
    //[Authorize]
    public class UserAppService : BaseApplicationService, IUserAppService
    {
        protected IdentityUserManager _userManager { get; }
        //protected IIdentityUserRepository _identityUserRepository { get; }
        //public IIdentityRoleRepository _roleRepository { get; }
        private readonly IRepository<User, Guid> _userRepository;
        private readonly IRepository<Organization, Guid> _orgRepository;
        private readonly IRepository<Position, Guid> _posRepository;
        private readonly IRepository<UserPosition> _userPositionsRepository;
        private readonly IRepository<UserOrganization> _userOrgsRepository;
        private readonly IRepository<Role, Guid> _roleRepository;
        private readonly IRepository<UserRole> _userRoleRepository;
        private readonly IRepository<RoleAuthority> _roleAuthorityRoleRepository;
        //private readonly IRepository<Authority, Guid> _authorityRepository;
        private readonly IConfiguration _configuration;
        //private readonly IDistributedCache<CurrentUserDto> _cache;


        public UserAppService(
            IdentityUserManager userManager,
            //IIdentityUserRepository identityUserRepository,
            //IIdentityRoleRepository roleRepository,
            IRepository<User, Guid> userRepository,
            IRepository<Organization, Guid> orgRepository,
            IRepository<Position, Guid> posRepository,
            IRepository<UserPosition> userPositionsRepository,
            IRepository<UserOrganization> userOrgsRepository,
            IRepository<Role, Guid> roleRepository,
            IRepository<UserRole> userRoleRepository,
            //IRepository<RoleAuthority> roleAuthorityRoleRepository,
            IConfiguration configuration,
             IDefaultAppService defaultAppService
            //IRepository<Authority, Guid> authorityRepository
            ) : base(defaultAppService)
        {
            _userManager = userManager;
            //_identityUserRepository = identityUserRepository;
            _roleRepository = roleRepository;
            _userRepository = userRepository;
            _orgRepository = orgRepository;
            _posRepository = posRepository;
            _userPositionsRepository = userPositionsRepository;
            _userOrgsRepository = userOrgsRepository;
            _userRoleRepository = userRoleRepository;
            //_roleAuthorityRoleRepository = roleAuthorityRoleRepository;
            _configuration = configuration;
            //_authorityRepository = authorityRepository;
        }



        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("create")]
        [Authorize(IdentityPermissions.Users.Create)]
        public async Task<ResultDto<Guid>> Create(BaseIdentityUserCreateDto input)
        {
            var result = new ResultDto<Guid>();

            var query = await _userRepository.FindAsync(p => p.UserName.Equals(input.UserName));
            if (query != null)
            {
                result.Message = $"账户{input.UserName}已存在，请更换账户名称！";
                return result;
            }

            var authorityId = CurrentAuthority.Id;

            var user = new IdentityUser(
                GuidGenerator.Create(),
                input.UserName,
                input.Email,
                CurrentTenant.Id
            );

            input.MapExtraPropertiesTo(user);

            user.ExtraProperties.Add(nameof(User.Sex), input.Sex);
            user.ExtraProperties.Add(nameof(User.JobNo), input.JobNo);
            user.ExtraProperties.Add(nameof(User.AuthorityId), authorityId);

            (await _userManager.CreateAsync(user, input.Password)).CheckErrors();
            await UpdateUserByInput(user, input);


            //var dto = ObjectMapper.Map<IdentityUser, BaseIdentityUserDto>(user);
            //dto.Sex = input.Sex;
            //dto.AuthorityId = authorityId;

            //dto.RootOrgId = input.RootOrgId;
            //var dto = ObjectMapper.Map<BaseIdentityUserCreateDto, BaseIdentityUserDto>(input);

            foreach (var id in input.PositionIds)
            {
                await _userPositionsRepository.InsertAsync(new UserPosition(CurrentTenant.Id, user.Id, id, authorityId));
            }

            foreach (var id in input.OrganizationIds)
            {
                await _userOrgsRepository.InsertAsync(new UserOrganization(CurrentTenant.Id, user.Id, id, authorityId));
            }

            //foreach (var id in input.Roles)
            //{
            //    await _userRoleRepository.InsertAsync(new UserRole(CurrentTenant.Id, user.Id, id));
            //}
            if (input.Roles != null)
            {
                List<UserRole> user_role = new List<UserRole>();
                foreach (var rid in input.Roles)
                {
                    user_role.Add(new UserRole(CurrentTenant.Id, user.Id, rid));
                }
                await _userRoleRepository.InsertManyAsync(user_role);
            }

            await CurrentUnitOfWork.SaveChangesAsync();

            result.SetData(user.Id);
            return result;
        }

        /// <summary>
        /// 更新用户
        /// </summary>     
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("update")]
        [Authorize(IdentityPermissions.Users.Update)]
        public async Task<ResultDto<bool>> Update(BaseIdentityUserUpdateDto input)
        {
            var result = new ResultDto<bool>();

            var user = await _userManager.GetByIdAsync(input.Id);
            user.ConcurrencyStamp = input.ConcurrencyStamp;

            (await _userManager.SetUserNameAsync(user, input.UserName)).CheckErrors();

            await UpdateUserByInput(user, input);
            input.MapExtraPropertiesTo(user);
            user.SetProperty(nameof(User.Sex), input.Sex);
            user.SetProperty(nameof(User.JobNo), input.JobNo);
            //user.SetProperty(nameof(User.AuthorityId), input.RootOrgId);

            (await _userManager.UpdateAsync(user)).CheckErrors();

            if (!input.Password.IsNullOrEmpty())
            {
                (await _userManager.RemovePasswordAsync(user)).CheckErrors();
                (await _userManager.AddPasswordAsync(user, input.Password)).CheckErrors();
            }

            // var dto = ObjectMapper.Map<IdentityUser, IdentityUserDto>(user);
            //var dto = ObjectMapper.Map<BaseIdentityUserUpdateDto, BaseIdentityUserDto>(input);

            await _userPositionsRepository.DeleteAsync(p => p.UserId == input.Id);

            var authorityId = new Guid(user.ExtraProperties[nameof(User.AuthorityId)].ToString());

            foreach (var jid in input.PositionIds)
            {
                await _userPositionsRepository.InsertAsync(new UserPosition(CurrentTenant.Id, input.Id, jid, authorityId));
            }

            await _userOrgsRepository.DeleteAsync(p => p.UserId == input.Id);

            foreach (var oid in input.OrganizationIds)
            {
                await _userOrgsRepository.InsertAsync(new UserOrganization(CurrentTenant.Id, input.Id, oid, authorityId));
            }

            if (input.Roles != null)
            {
                // 清空角色列表
                await _userRoleRepository.DeleteAsync(p => p.UserId == input.Id);

                // 循环增加角色
                //foreach (var rid in input.Roles)
                //{
                //    await _userRoleRepository.InsertAsync(new UserRole(CurrentTenant.Id, input.Id, rid));
                //}
                List<UserRole> user_role = new List<UserRole>();
                foreach (var rid in input.Roles)
                {
                    user_role.Add(new UserRole(CurrentTenant.Id, input.Id, rid));
                }
                await _userRoleRepository.InsertManyAsync(user_role);
            }
            await CurrentUnitOfWork.SaveChangesAsync();

            result.SetData(true);
            return result;
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("delete")]
        [Authorize(IdentityPermissions.Users.Delete)]
        public async Task<ResultDto<bool>> Delete(List<Guid> ids)
        {
            var result = new ResultDto<bool>();

            try
            {
                await _userRepository.DeleteManyAsync(ids);
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
        /// 查询实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<ResultDto<BaseIdentityUserDto>> Get(Guid id)
        {
            var result = new ResultDto<BaseIdentityUserDto>();

            //var dto = ObjectMapper.Map<User, BaseIdentityUserDto>(await UserManager.GetByIdAsync(id));
            var dto = ObjectMapper.Map<User, BaseIdentityUserDto>(await _userRepository.GetAsync(id));
            var jobIds = await (await _userPositionsRepository.GetQueryableAsync()).Where(p => p.UserId == id).Select(p => p.PositionId).ToListAsync();
            var orgIds = await (await _userOrgsRepository.GetQueryableAsync()).Where(p => p.UserId == id).Select(p => p.OrganizationId).ToListAsync();
            var roleIds = await (await _userRoleRepository.GetQueryableAsync()).Where(p => p.UserId == id).Select(p => p.RoleId).ToListAsync();
            dto.PositionIds = jobIds;
            dto.OrganizationIds = orgIds;
            dto.Roles = roleIds;

            result.SetData(dto);
            return result;
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("list")]
        public async Task<ResultDto<PagedResultDto<BaseIdentityUserDto>>> GetList(GetBaseIdentityUsersInput input)
        {
            var result = new ResultDto<PagedResultDto<BaseIdentityUserDto>>();
            var authorityId = CurrentAuthority.Id;

            //var orgQueryable = await _orgRepository.GetQueryableAsync();//机构信息
            //var userOrgQueryable = await _userOrgsRepository.GetQueryableAsync();//用户所属机构信息

            //var pos = await _posRepository.GetQueryableAsync();//岗位信息
            //var userPosQueryable = await _userPositionsRepository.GetQueryableAsync();//用户所属信息

            //var role = await _roleRepository.GetQueryableAsync();
            //var userRoleQueryable = await _userRoleRepository.GetQueryableAsync();

            List<User> items = new List<User>();
            long totalCount = 0;
            var userDbSet = (await _userRepository.GetQueryableAsync()).Where(p => p.AuthorityId == authorityId);
            if (input.OrganizationId.HasValue)
            {
                var userOrgQueryable = await _userOrgsRepository.GetQueryableAsync();//用户所属机构信息
                var org = await _orgRepository.GetAsync(input.OrganizationId.Value);
                var orgDbSet = await _orgRepository.GetQueryableAsync();
                var userIds = new List<Guid>();

                ////TODO: Redis Query
                //if (CurrentUser.UserName != BaseConsts.SuperAdmin)
                //{
                //    userDbSet = userDbSet.Where(p => p.AuthorityId == CurrentAuthority.Id);
                //    orgDbSet = orgDbSet.Where(p => p.CascadeId.Contains(org.CascadeId) && p.AuthorityId == CurrentAuthority.Id);
                //    userIds = await userOrgQueryable.Where(p => orgDbSet.Select(o => o.Id).Contains(p.OrganizationId) && p.AuthorityId == CurrentAuthority.Id)
                //                                      .Select(p => p.UserId)
                //                                      .Distinct()
                //                                      .Skip(input.SkipCount)
                //                                      .Take(input.MaxResultCount)
                //                                      .ToListAsync();
                //}
                //else
                //{
                //    orgDbSet = orgDbSet.Where(p => p.CascadeId.Contains(org.CascadeId));
                //    userIds = await userOrgQueryable.Where(p => orgDbSet.Select(o => o.Id).Contains(p.OrganizationId))
                //                                    .Select(p => p.UserId)
                //                                    .Distinct()
                //                                    .Skip(input.SkipCount)
                //                                    .Take(input.MaxResultCount)
                //                                    .ToListAsync();
                //}

                orgDbSet = orgDbSet.Where(p => p.CascadeId.Contains(org.CascadeId) && p.AuthorityId == authorityId);
                userIds = await userOrgQueryable.Where(p => orgDbSet.Select(o => o.Id).Contains(p.OrganizationId) && p.AuthorityId == authorityId)
                                                  .Select(p => p.UserId)
                                                  .Distinct()
                                                  .Skip(input.SkipCount)
                                                  .Take(input.MaxResultCount)
                                                  .ToListAsync();

                totalCount = await userOrgQueryable.Where(p => orgDbSet.Select(o => o.Id).Contains(p.OrganizationId))
                                               .GroupBy(p => p.UserId)
                                               .LongCountAsync();

                items = await userDbSet
                    .Where(p => userIds.Contains(p.Id))
                    .OrderByDescending(p => p.Id)
                    .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), p => p.UserName.Contains(input.Filter))
                    .ToListAsync();


            }
            else
            {
                //if (CurrentUser.UserName != BaseConsts.SuperAdmin)
                //{
                //    userDbSet = userDbSet.Where(p => p.AuthorityId == CurrentAuthority.Id);

                //}

                totalCount = await userDbSet.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), p => p.UserName.Contains(input.Filter))
                                            .CountAsync();

                items = await userDbSet
                    .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), p => p.UserName.Contains(input.Filter))
                    .OrderByDescending(p => p.Id)
                    .Skip(input.SkipCount)
                    .Take(input.MaxResultCount)
                    .ToListAsync();

            }

            var dtos = ObjectMapper.Map<List<User>, List<BaseIdentityUserDto>>(items);

            #region 机构信息
            //var userOrgs = await userOrgQueryable.Where(p => items.Select(i => i.Id).Contains(p.UserId)).ToListAsync();
            //var allOrg = await orgQueryable.Where(p => userOrgs.Select(uo => uo.OrganizationId).Contains(p.Id))
            //                               .OrderBy(p => p.CascadeId)
            //                               .ToListAsync();

            //foreach (var dto in dtos)
            //{
            //    var oids = userOrgs.Where(p => p.UserId == dto.Id).Select(p => p.OrganizationId).ToList();
            //    dto.OrganizationNames = string.Join(", ", allOrg.Where(p => oids.Contains(p.Id)).Select(p => p.Name).ToArray());
            //}
            #endregion

            #region 岗位信息

            //foreach (var dto in dtos)
            //{
            //    var pids = await userPosQueryable.Where(p => p.UserId == dto.Id).Select(p => p.PositionId).ToListAsync();
            //    dto.PositionNames = string.Join(", ", allOrg.Where(p => pids.Contains(p.Id)).Select(p => p.Name).ToArray());
            //}

            #endregion

            #region 角色信息
            //foreach (var dto in dtos)
            //{
            //    var rids = await userRoleQueryable.Where(p => p.UserId == dto.Id).Select(p => p.RoleId).ToListAsync();
            //    dto.RoleNames = string.Join(", ", allOrg.Where(p => rids.Contains(p.Id)).Select(p => p.Name).ToArray());
            //}
            #endregion

            var data = new PagedResultDto<BaseIdentityUserDto>(totalCount, dtos);
            result.SetData(data);
            return result;
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("profile")]
        public async Task<ResultDto<CurrentUserDto>> GetCurrentUser()
        {
            var result = new ResultDto<CurrentUserDto>();
           
            //var userId = CurrentUser.Id.Value;
            //var dto = ObjectMapper.Map<User, CurrentUserDto>(await _userRepository.GetAsync(userId));
            var dto = ObjectMapper.Map<ICurrentUser, CurrentUserDto>(CurrentUser);
            //var roleIds = await (await _userRoleRepository.GetQueryableAsync()).Where(p => p.UserId == userId).Select(p => p.RoleId).ToListAsync();
            //var roles = await _roleRepository.GetListAsync(p => roleIds.Contains(p.Id));
            //dto.Roles = roles.Select(p => p.Name).ToList();
            result.SetData(dto);
            return result;
        }
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("change-password")]
        public async Task<ResultDto<bool>> ChangeUserPassword(Guid id, ChangePasswordDto input)
        {
            var result = new ResultDto<bool>();

            var user = await _userManager.GetByIdAsync(id);
            try
            {
                (await _userManager.ChangePasswordAsync(user, input.CurrentPassword, input.NewPassword)).CheckErrors();

                result.SetData(true, message: "修改成功！");
            }
            catch (Exception ex)
            {
                result.SetData(false, (int)ResultCode.Fail, ex.Message);
            }

            return result;
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("reset-password")]
        public async Task<ResultDto<bool>> ResetUserPassword(List<Guid> ids)
        {
            var result = new ResultDto<bool>();
            try
            {
                foreach (var id in ids)
                {
                    var user = await _userManager.GetByIdAsync(id);

                    var resettoken = await _userManager.GeneratePasswordResetTokenAsync(user);

                    var secret = _configuration["App:DefaultPassword"];

                    (await _userManager.ResetPasswordAsync(user, resettoken, secret)).CheckErrors();
                }
                result.SetData(true, message: "重置成功！");
            }
            catch (Exception ex)
            {
                result.SetData(false, (int)ResultCode.Fail, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 密码校验
        /// </summary>
        /// <param name="PassWord"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("check-password")]
        public async Task<ResultDto<bool>> CheckPassWord(Guid userId, string PassWord)
        {
            var result = new ResultDto<bool>();

            if (string.IsNullOrWhiteSpace(PassWord))
            {
                result.SetData(false, (int)ResultCode.Fail, "密码不能为空");
                return result;
            }

            var user = await _userManager.GetByIdAsync(userId);
            if (user == null)
            {
                result.SetData(false, (int)ResultCode.Fail, "用户信息不存在");
                return result;
            }
            result.SetData(await _userManager.CheckPasswordAsync(user, PassWord));
            return result;
        }

        protected virtual async Task UpdateUserByInput(IdentityUser user, BaseIdentityUserCreateOrUpdateDto input)
        {
            if (input.Sex != null)
            {
                user.SetProperty(nameof(User.Sex), input.Sex);
            }
            if (input.JobNo != null)
            {
                user.SetProperty(nameof(User.JobNo), input.JobNo);
            }

            if (input.IsActive != null)
            {
                bool flag = input.IsActive == true ? true : false;
                user.SetIsActive(flag);
            }

            if (input.Email != null && !string.Equals(user.Email, input.Email, StringComparison.InvariantCultureIgnoreCase))
            {
                (await _userManager.SetEmailAsync(user, input.Email)).CheckErrors();
            }

            if (input.PhoneNumber != null && !string.Equals(user.PhoneNumber, input.PhoneNumber, StringComparison.InvariantCultureIgnoreCase))
            {
                (await _userManager.SetPhoneNumberAsync(user, input.PhoneNumber)).CheckErrors();
            }

            if (input.LockoutEnabled != null)
            {
                (await _userManager.SetLockoutEnabledAsync(user, (bool)input.LockoutEnabled)).CheckErrors();
            }

            if (input.Name != null)
            {
                user.Name = input.Name;
            }

            if (input.Surname != null)
            {
                user.Surname = input.Surname;
            }

            if (input.RoleNames != null)
            {
                (await _userManager.SetRolesAsync(user, input.RoleNames)).CheckErrors();
            }
        }

    }
}
