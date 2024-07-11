using BaseService.BaseData.PositionManagement.Dto;
using BaseService.Consts;
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

namespace BaseService.BaseData.PositionManagement
{
    /// <summary>
    /// 岗位
    /// </summary>
    [Area("Base")]
    [Route("api/base/position")]
    [Authorize]
    public class PositionAppService : BaseApplicationService, IPositionAppService
    {
        private readonly IRepository<Position, Guid> _repository;

        public PositionAppService(
             IRepository<Position, Guid> repository,
             IDefaultAppService defaultAppService
            ) : base(defaultAppService)
        {
            _repository = repository;
        }

        /// <summary>
        /// 创建岗位
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("create")]
        [Authorize(BaseServicePermissions.Position.Create)]
        public async Task<ResultDto<Guid>> Create(CreateOrUpdatePositionDto input)
        {
            var result = new ResultDto<Guid>();
            var authorityId = CurrentAuthority.Id;

            var exist = await _repository.FirstOrDefaultAsync(p => p.Name == input.Name && p.AuthorityId == authorityId);
            if (exist != null)
            {
                //throw new BusinessException("名称：" + input.Name + "岗位已存在");
                result.Message = "名称：" + input.Name + "岗位已存在";
                return result;
            }

            var position = new Position(GuidGenerator.Create(), CurrentTenant.Id, input.Name, input.IsActive, input.Sort, input.Description, CurrentAuthority.Id);

            var query = await _repository.InsertAsync(position);
            //var dto = ObjectMapper.Map<Position, PositionDto>(query);

            result.SetData(query.Id);
            return result;
        }

        /// <summary>
        /// 更新岗位
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("update")]
        [Authorize(BaseServicePermissions.Position.Update)]
        public async Task<ResultDto<bool>> Update(CreateOrUpdatePositionDto input)
        {
            var result = new ResultDto<bool>();
            var authorityId = CurrentAuthority.Id;

            var query = await _repository.FirstOrDefaultAsync(p => p.Name.Equals(input.Name) && p.Id != input.Id && p.AuthorityId == authorityId);
            if (query != null)
            {
                result.Message = $"名称:{input.Name},字典已存在";
                return result;
            }

            var job = await _repository.GetAsync(input.Id.Value);

            job.Name = input.Name;
            job.IsActive = input.IsActive;
            job.Sort = input.Sort;
            job.Description = input.Description;

            await _repository.UpdateAsync(job);

            result.SetData(true);
            return result;
        }

        /// <summary>
        /// 删除岗位
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("delete")]
        [Authorize(BaseServicePermissions.Position.Delete)]
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
        public async Task<ResultDto<PositionDto>> Get(Guid id)
        {
            var result = new ResultDto<PositionDto>();

            var query = await _repository.GetAsync(id);
            var dto = ObjectMapper.Map<Position, PositionDto>(query);

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
        public async Task<ResultDto<PagedResultDto<PositionDto>>> GetList(GetPositionInputDto input)
        {
            var result = new ResultDto<PagedResultDto<PositionDto>>();
            //var authorityId = CurrentAuthority.Id;

            var query = (await _repository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), p => p.Name.Contains(input.Filter));

            if (CurrentUser.UserName != SystemConsts.SuperAdmin)
                query = query.Where(p => p.AuthorityId == CurrentAuthority.Id);

            var totalCount = await query.CountAsync();
            var items = await query.OrderBy(input.Sorting ?? nameof(PositionDto.Name))
                                   .Skip(input.SkipCount)
                                   .Take(input.MaxResultCount)
                                   .ToListAsync();

            var list = ObjectMapper.Map<List<Position>, List<PositionDto>>(items);
            var data = new PagedResultDto<PositionDto>(totalCount, list);

            result.SetData(data);
            return result;
        }

        /// <summary>
        /// 查询所有岗位
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("all")]
        public async Task<ResultDto<ListResultDto<PositionDto>>> GetAllPositions(string filter)
        {
            var result = new ResultDto<ListResultDto<PositionDto>>();
            var authorityId = CurrentAuthority.Id;

            var jobs = await (await _repository.GetQueryableAsync())
                .Where(p => p.IsActive == true && p.AuthorityId == authorityId)
                .WhereIf(!string.IsNullOrWhiteSpace(filter), p => p.Name.Contains(filter))
                .ToListAsync();

            var dto = new ListResultDto<PositionDto>(ObjectMapper.Map<List<Position>, List<PositionDto>>(jobs));

            result.SetData(dto);
            return result;
        }


    }
}
