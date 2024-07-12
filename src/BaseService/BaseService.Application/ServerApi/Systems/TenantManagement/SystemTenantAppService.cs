using BaseService.Consts;
using BaseService.Systems.TenantManagement.Dto;
using Cimc.Helper;
using Cimc.Model.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Data;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.ObjectExtending;
using Volo.Abp.TenantManagement;

namespace BaseService.Systems.TenantManagement
{
    /// <summary>
    /// 租户
    /// </summary>

    [Area("Base")]
    [Route("api/base/tenant")]
    [Authorize(TenantManagementPermissions.Tenants.Default)]
    public class SystemTenantAppService : ApplicationService, ISystemTenantAppService
    {
        protected IDataSeeder DataSeeder { get; }
        protected ITenantRepository TenantRepository { get; }
        protected ITenantManager TenantManager { get; }
        protected IDistributedEventBus DistributedEventBus { get; }
        protected IdentityUserManager _userManager { get; }

        public SystemTenantAppService(
            ITenantRepository tenantRepository,
            ITenantManager tenantManager,
            IDataSeeder dataSeeder,
            IDistributedEventBus distributedEventBus,
             IdentityUserManager userManager
          )
        {
            DataSeeder = dataSeeder;
            TenantRepository = tenantRepository;
            TenantManager = tenantManager;
            DistributedEventBus = distributedEventBus;
            _userManager = userManager;
        }

        /// <summary>
        /// 查询实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<ResultDto<TenantDto>> Get(Guid id)
        {
            var result = new ResultDto<TenantDto>();
            var entity = await TenantRepository.GetAsync(id);
            var dto = ObjectMapper.Map<Tenant, TenantDto>(entity);

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
        public async Task<ResultDto<PagedResultDto<TenantDto>>> GetList(GetSystemTenantInput input)
        {
            var result = new ResultDto<PagedResultDto<TenantDto>>();

            if (input.Sorting.IsNullOrWhiteSpace())
            {
                input.Sorting = nameof(Tenant.Name);
            }

            var count = await TenantRepository.GetCountAsync(input.Filter);
            var list = await TenantRepository.GetListAsync(
                input.Sorting,
                input.MaxResultCount,
                input.SkipCount,
                input.Filter
            );

            var data = new PagedResultDto<TenantDto>(
                  count,
                  ObjectMapper.Map<List<Tenant>, List<TenantDto>>(list)
              );

            result.SetData(data);
            return result;
        }

        /// <summary>
        /// 创建租户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("create")]
        [Authorize(TenantManagementPermissions.Tenants.Create)]
        public async Task<ResultDto<Guid>> Create(TenantCreateDto input)
        {
            var result = new ResultDto<Guid>();

            var exist = (await TenantRepository.GetListAsync()).Where(p => p.Name.Equals(input.Name)).FirstOrDefault();
            if (exist != null)
            {
                result.Message = "名称：" + input.Name + "，租户已存在";
                return result;
            }

            var tenant = await TenantManager.CreateAsync(input.Name);
            input.MapExtraPropertiesTo(tenant);

            var entity = await TenantRepository.InsertAsync(tenant);

            await CurrentUnitOfWork.SaveChangesAsync();

            await DistributedEventBus.PublishAsync(
                new TenantCreatedEto
                {
                    Id = tenant.Id,
                    Name = tenant.Name,
                    Properties =
                    {
                        { "AdminEmail", input.AdminEmailAddress },
                        { "AdminPassword", input.AdminPassword }
                    }
                });

            using (CurrentTenant.Change(tenant.Id, tenant.Name))
            {
                //TODO: Handle database creation?
                // TODO: Seeder might be triggered via event handler.
                await DataSeeder.SeedAsync(
                                new DataSeedContext(tenant.Id)
                                    .WithProperty("AdminEmail", input.AdminEmailAddress)
                                    .WithProperty("AdminPassword", input.AdminPassword)
                                );
            }

            //ObjectMapper.Map<Tenant, TenantDto>(tenant);
            result.SetData(tenant.Id);
            return result;
        }


        /// <summary>
        /// 更新租户
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("update")]
        [Authorize(TenantManagementPermissions.Tenants.Update)]
        public async Task<ResultDto<bool>> Update(SystemTenantUpdateDto input)
        {
            var result = new ResultDto<bool>();

            var tenant = await TenantRepository.GetAsync(input.Id);

            #region 扩展逻辑

            if (input.Name != tenant.Name)
            {
                result.Message = "租户名称不可修改！";
                return result;
            }

            if (string.IsNullOrWhiteSpace(input.AdminPassword))
            {
                result.Message = "租户密码不能为空！";
                return result;
            }

            if (!string.IsNullOrWhiteSpace(input.AdminEmailAddress) && !StringHelper.IsEmail(input.AdminEmailAddress))
            {
                result.Message = "邮箱格式错误！";
                return result;
            }

            //切换租户
            using (CurrentTenant.Change(tenant.Id))
            {
                var user = await _userManager.FindByNameAsync(SystemConsts.SuperAdmin);
                var resettoken = await _userManager.GeneratePasswordResetTokenAsync(user);
                var secret = input.AdminPassword;
                (await _userManager.ResetPasswordAsync(user, resettoken, secret)).CheckErrors();

                if (!string.IsNullOrWhiteSpace(input.AdminEmailAddress))
                    (await _userManager.SetEmailAsync(user, input.AdminEmailAddress)).CheckErrors();
            }

            #endregion

            //await TenantManager.ChangeNameAsync(tenant, tenant.Name);
            //tenant.SetConcurrencyStampIfNotNull(input.ConcurrencyStamp);
            //input.MapExtraPropertiesTo(tenant);
            //await TenantRepository.UpdateAsync(tenant);

            result.SetData(true, message: "修改成功！");
            return result;
        }

        /// <summary>
        /// 删除租户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("delete")]
        [Authorize(TenantManagementPermissions.Tenants.Delete)]
        public async Task<ResultDto<bool>> Delete(Guid id)
        {
            var result = new ResultDto<bool>();

            var tenant = await TenantRepository.FindAsync(id);
            if (tenant == null)
            {
                result.SetData(false, message: "租户不存在");
                return result;
            }

            await TenantRepository.DeleteAsync(tenant);
            result.SetData(true);
            return result;
        }

        /// <summary>
        /// 获取所有租户
        /// </summary>
        /// <returns></returns>
        /// <summary>
        /// 获取所有租户名称 （免登录）
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("all-names")]
        public async Task<ResultDto<ListResultDto<string>>> GetAllNames()
        {
            var result = new ResultDto<ListResultDto<string>>();

            var list = (await TenantRepository.GetListAsync()).Select(p => p.Name).ToList();

            var data = new ListResultDto<string>(list);

            result.SetData(data);
            return result;
        }

        //[Authorize(TenantManagementPermissions.Tenants.ManageConnectionStrings)]
        //public virtual async Task<string> GetDefaultConnectionStringAsync(Guid id)
        //{
        //    var tenant = await TenantRepository.GetAsync(id);
        //    return tenant?.FindDefaultConnectionString();
        //}

        //[Authorize(TenantManagementPermissions.Tenants.ManageConnectionStrings)]
        //public virtual async Task UpdateDefaultConnectionStringAsync(Guid id, string defaultConnectionString)
        //{
        //    var tenant = await TenantRepository.GetAsync(id);
        //    tenant.SetDefaultConnectionString(defaultConnectionString);
        //    await TenantRepository.UpdateAsync(tenant);
        //}

        //[Authorize(TenantManagementPermissions.Tenants.ManageConnectionStrings)]
        //public virtual async Task DeleteDefaultConnectionStringAsync(Guid id)
        //{
        //    var tenant = await TenantRepository.GetAsync(id);
        //    tenant.RemoveDefaultConnectionString();
        //    await TenantRepository.UpdateAsync(tenant);
        //}
    }
}
