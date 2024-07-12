using BaseService.BaseData;
using BaseService.BaseData.UserFeatureManagement;
using BaseService.BaseData.UserFeatureManagement.Dto;
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
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace BaseService.ServerApi.BaseData.UserFeatureManagement
{
    /// <summary>
    /// 用户特征
    /// </summary>
    [Area("Base")]
    [Route("api/base/user-feature")]
    [Authorize]
    public class UserFeatureAppService : ApplicationService, IUserFeatureAppService
    {
        private readonly IRepository<UserFeature, Guid> _repository;

        public UserFeatureAppService(
                IRepository<UserFeature, Guid> repository
            )
        {
            _repository = repository;
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("create")]
        public async Task<ResultDto<Guid?>> Create(CreateOrUpdateUserFeatureDto input)
        {
            var result = new ResultDto<Guid?>();

            var query = await _repository.FindAsync(p => p.DataKey.Equals(input.DataKey));

            if (query != null)
            {
                result.Message = $"{input.DataKey}已存在";
                return result;
            }

            input.Id = GuidGenerator.Create();
            var model = ObjectMapper.Map<CreateOrUpdateUserFeatureDto, UserFeature>(input);
            model.UserId = CurrentUser.Id.Value;

            var entiey = await _repository.InsertAsync(model);
            await CurrentUnitOfWork.SaveChangesAsync();

            result.SetData(entiey.Id);
            return result;
        }

        /// <summary>
        /// 更新
        /// </summary>      
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("update")]
        public async Task<ResultDto<bool>> Update(CreateOrUpdateUserFeatureDto input)
        {
            var result = new ResultDto<bool>();

            var query = await _repository.FindAsync(p => p.DataKey.Equals(input.DataKey) && p.Id != input.Id);
            if (query != null)
            {
                result.Message = $"{input.DataKey}已存在";
                return result;
            }

            //var model = ObjectMapper.Map<CreateOrUpdateUserFeatureDto, UserFeature>(input);
            var model = await _repository.FindAsync(input.Id.Value);
            model.Name = input.Name;
            model.DataKey = input.DataKey;
            model.DataValue = input.DataValue;
            model.UserId = CurrentUser.Id.Value;

            await _repository.UpdateAsync(model);
            await CurrentUnitOfWork.SaveChangesAsync();

            result.SetData(true);
            return result;
        }


        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("delete")]
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
        /// 通过数据ID查询单个实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<ResultDto<UserFeatureDto>> Get(Guid id)
        {
            var result = new ResultDto<UserFeatureDto>();

            var query = await _repository.FindAsync(id);
            if (query != null)
            {
                var dto = ObjectMapper.Map<UserFeature, UserFeatureDto>(query);
                result.SetData(dto);
            }
            else
            {
                result.SetData(message: "数据不存在！");
            }
            return result;
        }

        /// <summary>
        /// 通过数据健值查询单个实体
        /// </summary>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("get")]
        public async Task<ResultDto<UserFeatureDto>> Get(string dataKey)
        {
            var result = new ResultDto<UserFeatureDto>();

            var query = await _repository.FindAsync(p => p.DataKey.Equals(dataKey));
            if (query != null)
            {
                var dto = ObjectMapper.Map<UserFeature, UserFeatureDto>(query);
                result.SetData(dto);
            }
            else
            {
                result.SetData(message: "数据不存在！");
            }

            return result;
        }

        /// <summary>
        /// 分页查询列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("list")]
        public async Task<ResultDto<PagedResultDto<UserFeatureDto>>> GetList(GetUserFeatureInputDto input)
        {
            var result = new ResultDto<PagedResultDto<UserFeatureDto>>();


            var query = (await _repository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), p => p.Name.Contains(input.Filter) || p.DataKey.Contains(input.Filter));


            var items = await query.OrderBy(input.Sorting ?? nameof(UserFeature.CreationTime) + " desc")
                                 .Skip(input.SkipCount)
                                 .Take(input.MaxResultCount)
                                 .ToListAsync();
            var totalCount = await query.CountAsync();

            var list = ObjectMapper.Map<List<UserFeature>, List<UserFeatureDto>>(items);

            var data = new PagedResultDto<UserFeatureDto>(totalCount, list);

            result.SetData(data);
            return result;
        }


    }
}
