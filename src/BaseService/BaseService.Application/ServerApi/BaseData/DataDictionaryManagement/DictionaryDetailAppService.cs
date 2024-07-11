
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
using Volo.Abp.Domain.Repositories;

namespace BaseService.BaseData.DataDictionaryManagement
{
    /// <summary>
    /// 字典表子集
    /// </summary>
    [Area("Base")]
    [Route("api/base/dictDetails")]
    [Authorize]
    public class DictionaryDetailAppService : BaseApplicationService, IDictionaryDetailAppService
    {
        private readonly IRepository<DataDictionary, Guid> _masterRepository;
        private readonly IRepository<DataDictionaryDetail, Guid> _detailRepository;

        public DictionaryDetailAppService(
             IDefaultAppService defaultAppService,
            IRepository<DataDictionaryDetail, Guid> detailRepository,
            IRepository<DataDictionary, Guid> masterRepository) : base(defaultAppService)
        {
            _detailRepository = detailRepository;
            _masterRepository = masterRepository;
        }

        /// <summary>
        /// 创建字典子集
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("create")]
        [Authorize(BaseServicePermissions.DataDictionary.Create)]
        public async Task<ResultDto<Guid>> Create(CreateOrUpdateDictionaryDetailDto input)
        {
            var result = new ResultDto<Guid>();
            var authorityId = CurrentAuthority.Id;

            var master = await _masterRepository.FirstOrDefaultAsync(p => p.Id == input.DictionaryId && p.AuthorityId == authorityId);
            if (master == null)
            {
                //throw new BusinessException("未找到字典！");
                result.Message = "未找到字典！";
                return result;
            }

            var exist = await _detailRepository.FirstOrDefaultAsync(p => p.Label == input.Label && p.DictionaryId == master.Id && p.AuthorityId == authorityId);
            if (exist != null)
            {
                //throw new BusinessException("名称：" + input.Label + "字典已存在");
                result.Message = "名称：" + input.Label + "字典已存在";
                return result;
            }

            var query = await _detailRepository.InsertAsync(new DataDictionaryDetail(
                                                                    GuidGenerator.Create(),
                                                                    CurrentTenant.Id,
                                                                    input.DictionaryId,
                                                                    input.Label?.Trim(),
                                                                    input.Value?.Trim(),
                                                                    input.Sort,
                                                                    CurrentAuthority.Id));

            //var dto = ObjectMapper.Map<DataDictionaryDetail, DictionaryDetailDto>(query);

            result.SetData(query.Id);
            return result;
        }

        /// <summary>
        /// 更新字典子集
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("update")]
        [Authorize(BaseServicePermissions.DataDictionary.Update)]
        public async Task<ResultDto<bool>> Update(CreateOrUpdateDictionaryDetailDto input)
        {
            var result = new ResultDto<bool>();
            var authorityId = CurrentAuthority.Id;

            var exist = await _detailRepository.FirstOrDefaultAsync(p => p.Label == input.Label && p.DictionaryId == input.DictionaryId && p.Id != input.Id && p.AuthorityId == authorityId);
            if (exist != null)
            {
                result.Message = "名称：" + input.Label + "字典已存在";
                return result;
            }

            var detail = await _detailRepository.GetAsync(input.Id.Value);
            detail.Label = input.Label?.Trim();
            detail.Value = input.Value?.Trim();
            detail.Sort = input.Sort;

            await _detailRepository.UpdateAsync(detail);

            result.SetData(true);
            return result;
        }

        /// <summary>
        /// 删除字典子集
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
                await _detailRepository.DeleteManyAsync(ids);
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
        public async Task<ResultDto<DictionaryDetailDto>> Get(Guid id)
        {
            var result = new ResultDto<DictionaryDetailDto>();

            var query = await _detailRepository.GetAsync(id);
            var dto = ObjectMapper.Map<DataDictionaryDetail, DictionaryDetailDto>(query);

            result.SetData(dto);
            return result;
        }

        /// <summary>
        /// 查询分页列表
        /// </summary>
        /// <param name="input">分页条件</param>
        /// <returns></returns>
        [HttpGet]
        [Route("list")]
        public async Task<ResultDto<PagedResultDto<DictionaryDetailDto>>> GetList(GetDictionaryDetailInputDto input)
        {
            var result = new ResultDto<PagedResultDto<DictionaryDetailDto>>();
            var authorityId = CurrentAuthority.Id;

            var query = (await _detailRepository.GetQueryableAsync())
                .Where(p => p.DictionaryId == input.DictionaryId && p.AuthorityId == authorityId);

            //if (CurrentUser.UserName != BaseConsts.SuperAdmin)
            //    query = query.Where(p => p.AuthorityId == CurrentAuthority.Id);

            var items = await query.OrderBy(input.Sorting ?? nameof(DataDictionaryDetail.Sort))
                     .Skip(input.SkipCount)
                     .Take(input.MaxResultCount)
                     .ToListAsync();

            var list = ObjectMapper.Map<List<DataDictionaryDetail>, List<DictionaryDetailDto>>(items);
            var totalCount = await query.CountAsync();
            var data = new PagedResultDto<DictionaryDetailDto>(totalCount, list);

            result.SetData(data);
            return result;
        }

        /// <summary>
        /// 查询子集合列表
        /// </summary>
        /// <param name="name">父级字典名称</param>
        /// <returns></returns>
        [HttpGet]
        [Route("all")]
        public async Task<ResultDto<ListResultDto<DictionaryDetailDto>>> GetAllByDictionaryName(string name)
        {
            var result = new ResultDto<ListResultDto<DictionaryDetailDto>>();
            var authorityId = CurrentAuthority.Id;

            var master = await _masterRepository.FirstOrDefaultAsync(p => p.Name == name.Trim() && p.AuthorityId == authorityId);

            if (master != null)
            {
                var details = await (await _detailRepository.GetQueryableAsync())
                                .Where(p => p.DictionaryId == master.Id)
                                .OrderBy(p => p.Sort)
                                .ToListAsync();

                if (details.Any())
                {
                    var dto = new ListResultDto<DictionaryDetailDto>(ObjectMapper.Map<List<DataDictionaryDetail>, List<DictionaryDetailDto>>(details));
                    result.SetData(dto);
                }
                else
                {
                    result.SetData(message: "字典值为空，请先添加字典值！");
                }
            }
            else
            {
                result.SetData(message: "字典不存在");
            }
            return result;
        }


    }
}
