using BaseService.Permissions;
using BaseService.Systems;
using BaseService.Systems.AuthorityManagerment;
using BaseService.Systems.AuthorityManagerment.Dto;
using Cimc.Model.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Mvc;

namespace BaseService.ServerApi.Systems.AuthorityManagerment
{
    [Area("Base")]
    [Route("api/base/authority-group")]
    [Authorize(BaseServicePermissions.Authority.Default)]
    public class AuthorityGroupAppService : ApplicationService, IAuthorityGroupAppService
    {
        private readonly IRepository<AuthorityGroup, Guid> _repository;

        public AuthorityGroupAppService(
            IRepository<AuthorityGroup, Guid> repository

            )
        {
            _repository = repository;
        }

        /// <summary>
        /// 创建权限对象分组
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("create")]
        [Authorize(BaseServicePermissions.Authority.Create)]
        public async Task<ResultDto<Guid>> Create(CreateOrUpdateAuthorityGroupDto input)
        {
            var result = new ResultDto<Guid>();

            var exist = await _repository.FirstOrDefaultAsync(p => p.GroupName == input.GroupName);
            if (exist != null)
            {
                result.Message = $"权限名称:{input.GroupName},已存在";
                return result;
            }

            var entity = new AuthorityGroup(
                        GuidGenerator.Create(),
                        input.GroupName,
                        CurrentTenant.Id
               );

            var query = await _repository.InsertAsync(entity);

            result.SetData(query.Id);
            return result;
        }

        /// <summary>
        /// 更新权限对象分组
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("update")]
        [Authorize(BaseServicePermissions.Authority.Update)]
        public async Task<ResultDto<bool>> Update(CreateOrUpdateAuthorityGroupDto input)
        {
            var result = new ResultDto<bool>();
            var detail = await _repository.GetAsync(input.Id.Value);
            detail.GroupName = input.GroupName;

            await _repository.UpdateAsync(detail);

            result.SetData(true);
            return result;
        }

        /// <summary>
        /// 删除权限对象分组
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("delete")]
        [Authorize(BaseServicePermissions.Authority.Delete)]
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
        public async Task<ResultDto<AuthorityGroupDto>> Get(Guid id)
        {
            var result = new ResultDto<AuthorityGroupDto>();

            var query = await _repository.GetAsync(id);
            var dto = ObjectMapper.Map<AuthorityGroup, AuthorityGroupDto>(query);

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
        public async Task<ResultDto<PagedResultDto<AuthorityGroupDto>>> GetList(GetAuthorityGroupInputDto input)
        {
            var result = new ResultDto<PagedResultDto<AuthorityGroupDto>>();

            var query = (await _repository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), p => p.GroupName.Contains(input.Filter));

            var items = await query.OrderBy(input.Sorting ?? "Id")
                     .Skip(input.SkipCount)
                     .Take(input.MaxResultCount)
                     .ToListAsync();
            var totalCount = await query.CountAsync();

            var list = ObjectMapper.Map<List<AuthorityGroup>, List<AuthorityGroupDto>>(items);
            var data = new PagedResultDto<AuthorityGroupDto>(totalCount, list);

            result.SetData(data);
            return result;
        }

        /// <summary>
        /// 查询全部
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("all")]
        public async Task<ResultDto<ListResultDto<AuthorityGroupDto>>> GetAll()
        {
            var result = new ResultDto<ListResultDto<AuthorityGroupDto>>();

            var items = await _repository.GetListAsync();

            var list = ObjectMapper.Map<List<AuthorityGroup>, List<AuthorityGroupDto>>(items);
            var data = new ListResultDto<AuthorityGroupDto>(list);

            result.SetData(data);
            return result;
        }

    }
}
