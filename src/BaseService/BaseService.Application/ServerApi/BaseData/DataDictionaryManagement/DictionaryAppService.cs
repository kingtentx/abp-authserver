using BaseService.BaseData.DataDictionaryManagement.Dto;
using BaseService.Permissions;
using BaseService.Systems;
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
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Repositories;

namespace BaseService.BaseData.DataDictionaryManagement
{
    /// <summary>
    /// 字典
    /// </summary>
    [Area("Base")]
    [Route("api/base/dict")]
    [Authorize]
    [DisableAuditing]
    public class DictionaryAppService : BaseApplicationService, IDictionaryAppService
    {
        private readonly IRepository<DataDictionary, Guid> _repository;

        public DictionaryAppService(
            IRepository<DataDictionary, Guid> repository,
            IDefaultAppService defaultAppService
            ) : base(defaultAppService)
        {
            _repository = repository;
        }

        /// <summary>
        /// 创建字典
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("create")]
        [Authorize(BaseServicePermissions.DataDictionary.Create)]
        public async Task<ResultDto<Guid>> Create(CreateOrUpdateDictionaryDto input)
        {
            var result = new ResultDto<Guid>();
            var authorityId = CurrentAuthority.Id;

            var exist = await _repository.FirstOrDefaultAsync(p => p.Name == input.Name && p.AuthorityId == authorityId);

            if (exist != null)
            {
                result.Message = $"名称:{input.Name},字典已存在";
                return result;
            }

            var dic = new DataDictionary(
                    GuidGenerator.Create(),
                    CurrentTenant.Id,
                    input.Name?.Trim(),
                    input.Description?.Trim(),
                    authorityId
                );

            var query = await _repository.InsertAsync(dic);

            result.SetData(dic.Id);
            return result;

        }

        /// <summary>
        /// 更新字典
        /// </summary>    
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("update")]
        [Authorize(BaseServicePermissions.DataDictionary.Update)]
        public async Task<ResultDto<bool>> Update(CreateOrUpdateDictionaryDto input)
        {
            var result = new ResultDto<bool>();
            var authorityId = CurrentAuthority.Id;

            var query = await _repository.FirstOrDefaultAsync(p => p.Name.Equals(input.Name) && p.Id != input.Id && p.AuthorityId == authorityId);
            if (query != null)
            {
                result.Message = $"名称:{input.Name},字典已存在";
                return result;
            }

            var dic = await _repository.GetAsync(input.Id.Value);
            dic.Name = input.Name?.Trim();
            dic.Description = input.Description?.Trim();
            await _repository.UpdateAsync(dic);

            result.SetData(true);
            return result;
        }

        /// <summary>
        /// 删除字典
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("delete")]
        [Authorize(BaseServicePermissions.DataDictionary.Delete)]
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
        public async Task<ResultDto<DictionaryDto>> Get(Guid id)
        {
            var result = new ResultDto<DictionaryDto>();

            var query = await _repository.GetAsync(id);
            var dto = ObjectMapper.Map<DataDictionary, DictionaryDto>(query);

            result.SetData(dto);
            return result;
        }

        /// <summary>
        /// 分页查询列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("list")]
        public async Task<ResultDto<PagedResultDto<DictionaryDto>>> GetList(GetDictionaryInputDto input)
        {
            var result = new ResultDto<PagedResultDto<DictionaryDto>>();
            var authorityId = CurrentAuthority.Id;

            var query = (await _repository.GetQueryableAsync())
                .Where(p => p.AuthorityId == authorityId)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), p => p.Name.Contains(input.Filter) || p.Description.Contains(input.Filter));

            //if (CurrentUser.UserName != BaseConsts.SuperAdmin)
            //    query = query.Where(p => p.AuthorityId == CurrentAuthority.Id);

            var items = await query.OrderBy(input.Sorting ?? nameof(DataDictionary.Name))
                                 .Skip(input.SkipCount)
                                 .Take(input.MaxResultCount)
                                 .ToListAsync();
            var totalCount = await query.CountAsync();

            var list = ObjectMapper.Map<List<DataDictionary>, List<DictionaryDto>>(items);

            var data = new PagedResultDto<DictionaryDto>(totalCount, list);

            result.SetData(data);
            return result;
        }


    }
}
