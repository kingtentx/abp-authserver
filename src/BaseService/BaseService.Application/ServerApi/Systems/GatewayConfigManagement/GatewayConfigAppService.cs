using BaseService.Consts;
using BaseService.Permissions;
using BaseService.Systems.AuthorityManagerment.Dto;
using BaseService.Systems.EdgeConfigManagement.Dto;
using Cimc.Model.Base;
using Microsoft.AspNetCore.Authorization;
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
using Volo.Abp.Domain.Repositories;

namespace BaseService.Systems.EdgeConfigManagement
{
    [Area("Base")]
    [Route("api/base/gateway-config")]
    [Authorize(BaseServicePermissions.Edge.Default)]
    public class GatewayConfigAppService : BaseApplicationService, IGatewayConfigAppService
    {
        private readonly IRepository<GatewayConfig, Guid> _repository;
        private readonly IDistributedCache<AuthorityConfigDto> _cache;
        public GatewayConfigAppService(IRepository<GatewayConfig, Guid> repository,
            IDistributedCache<AuthorityConfigDto> cache,
             IDefaultAppService defaultAppService) : base(defaultAppService)
        {
            _repository = repository;
            _cache = cache;
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("create")]
        [Authorize(BaseServicePermissions.Edge.Create)]
        public async Task<ResultDto<Guid>> Create(CreateOrUpdateGatewayConfigDto input)
        {
            var result = new ResultDto<Guid>();

            var exist = await _repository.FirstOrDefaultAsync(p => p.AppId == input.AppId);
            if (exist != null)
            {
                result.Message = $"appid:{input.Name},已存在";
                return result;
            }

            var entity = new GatewayConfig(
                        GuidGenerator.Create(),
                        CurrentTenant.Id,
                        input.Name,
                        input.Address,
                        input.AppId,
                        input.AppSecret,
                        input.Remark,
                        input.IsActive,
                        input.AuthorityId,
                        input.ServiceValue
               );

            var query = await _repository.InsertAsync(entity);

            result.SetData(query.Id);
            return result;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("delete")]
        [Authorize(BaseServicePermissions.Edge.Delete)]
        public async Task<ResultDto<bool>> Delete(List<Guid> ids)
        {
            var result = new ResultDto<bool>();
            try
            {
                await _repository.DeleteManyAsync(ids);
                await CurrentUnitOfWork.SaveChangesAsync();

                result.SetData(true);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                //result.SetData(false);
            }
            return result;
        }

        /// <summary>
        /// 查询单个实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<ResultDto<GatewayConfigDto>> Get(Guid id)
        {
            var result = new ResultDto<GatewayConfigDto>();

            var query = await _repository.GetAsync(id);
            var dto = ObjectMapper.Map<GatewayConfig, GatewayConfigDto>(query);

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
        public async Task<ResultDto<PagedResultDto<GatewayConfigDto>>> GetList(GetGatewayConfigInputDto input)
        {
            var result = new ResultDto<PagedResultDto<GatewayConfigDto>>();

            var query = (await _repository.GetQueryableAsync());

            if (CurrentUser.UserName != SystemConsts.SuperAdmin)
                query = query.Where(p => p.AuthorityId == CurrentAuthority.Id);

            var items = await query.OrderBy(input.Sorting ?? nameof(GatewayConfig.CreationTime) + " desc")
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
                .ToListAsync();

            var list = ObjectMapper.Map<List<GatewayConfig>, List<GatewayConfigDto>>(items);
            var totalCount = await query.CountAsync();
            var data = new PagedResultDto<GatewayConfigDto>(totalCount, list);

            result.SetData(data);
            return result;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("update")]
        [Authorize(BaseServicePermissions.Edge.Update)]
        public async Task<ResultDto<bool>> Update(CreateOrUpdateGatewayConfigDto input)
        {
            var result = new ResultDto<bool>();

            var exist = await _repository.FirstOrDefaultAsync(p => p.AppId == input.AppId && p.Id != input.Id);
            if (exist != null)
            {
                result.Message = $"appid:{input.Name},已存在";
                return result;
            }

            var entity = await _repository.GetAsync(input.Id.Value);
            entity.Name = input.Name;
            entity.Address = input.Address;
            entity.AppId = input.AppId;
            entity.AppSecret = input.AppSecret;
            entity.Remark = input.Remark;
            entity.IsActive = input.IsActive;
            entity.AuthorityId = input.AuthorityId;
            entity.ServiceValue = input.ServiceValue;

            await _repository.UpdateAsync(entity);

            await _cache.RemoveAsync(CacheConsts.AuthorityEdgeConfig + entity.AppId); //清除缓存 对应DefaultAppService.GetAuthorityEdgeConfig(string appId)
            //await _cache.RemoveAsync(CacheConsts.AuthorityEdgeConfig + "*"); //清除缓存

            result.SetData(true);
            return result;
        }
    }
}
