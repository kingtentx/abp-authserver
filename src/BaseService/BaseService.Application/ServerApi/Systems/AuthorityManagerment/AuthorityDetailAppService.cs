using BaseService.Permissions;
using BaseService.Systems.AuthorityManagerment.Dto;
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

namespace BaseService.Systems.AuthorityManagerment
{
    [Area("Base")]
    [Route("api/base/authority-detail")]
    [Authorize]
    public class AuthorityDetailAppService : ApplicationService, IAuthorityDetailAppService
    {
        private readonly IRepository<AuthorityDetail, Guid> _repository;
        //private readonly IRepository<Authority, Guid> _authrotyRepository;
        private readonly IRepository<RoleAuthority> _roleAuthorityRepository;
        private readonly IRepository<UserRole> _userRoleRepository;

        public AuthorityDetailAppService(
            IRepository<AuthorityDetail, Guid> repository,
            //IRepository<Authority, Guid> authrotyRepository,
            IRepository<RoleAuthority> roleAuthorityRepository,
            IRepository<UserRole> userRoleRepository)
        {
            _repository = repository;
            //_authrotyRepository = authrotyRepository;
            _roleAuthorityRepository = roleAuthorityRepository;
            _userRoleRepository = userRoleRepository;
        }

        /// <summary>
        /// 创建权限对象集合
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("create")]
        [Authorize(BaseServicePermissions.Authority.Create)]
        public async Task<ResultDto<Guid>> Create(CreateOrUpdateAuthorityDetailDto input)
        {
            var result = new ResultDto<Guid>();

            var entity = new AuthorityDetail(
                GuidGenerator.Create(),
                CurrentTenant.Id,
                input.AuthorityId,
                input.DataItemId,
                input.DataName,
                input.DataType
           );

            var query = await _repository.InsertAsync(entity);

            result.SetData(query.Id);
            return result;
        }

        /// <summary>
        /// 删除权限对象集合
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
        ///  查询单个实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<ResultDto<AuthorityDetailDto>> Get(Guid id)
        {
            var result = new ResultDto<AuthorityDetailDto>();

            var query = await _repository.GetAsync(id);
            var dto = ObjectMapper.Map<AuthorityDetail, AuthorityDetailDto>(query);

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
        public async Task<ResultDto<PagedResultDto<AuthorityDetailDto>>> GetList(GetAuthorityDetailInputDto input)
        {
            var result = new ResultDto<PagedResultDto<AuthorityDetailDto>>();

            var query = (await _repository.GetQueryableAsync()).Where(_ => _.AuthorityId == input.AuthorityId);
            var items = await query.OrderBy(input.Sorting ?? nameof(AuthorityDetail.CreationTime) + " desc")
                                   .Skip(input.SkipCount)
                                   .Take(input.MaxResultCount)
                                   .ToListAsync(); ;

            var list = ObjectMapper.Map<List<AuthorityDetail>, List<AuthorityDetailDto>>(items);
            var totalCount = await query.CountAsync();
            var data = new PagedResultDto<AuthorityDetailDto>(totalCount, list);

            result.SetData(data);
            return result;
        }

        /// <summary>
        /// 更新权限对象集合
        /// </summary>    
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("update")]
        [Authorize(BaseServicePermissions.Authority.Update)]
        public async Task<ResultDto<bool>> Update(CreateOrUpdateAuthorityDetailDto input)
        {
            var result = new ResultDto<bool>();
            var detail = await _repository.GetAsync(input.Id.Value);
            detail.AuthorityId = input.AuthorityId;
            detail.DataName = input.DataName;
            detail.DataItemId = input.DataItemId;
            detail.DataType = input.DataType;

            await _repository.UpdateAsync(detail);

            result.SetData(true);
            return result;
        }

        /// <summary>
        /// 获取当前权限下所有的子集
        /// </summary>
        /// <param name="authrotyId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("all")]
        public async Task<ResultDto<ListResultDto<AuthorityDetailDto>>> GetAll(Guid authrotyId)
        {
            var result = new ResultDto<ListResultDto<AuthorityDetailDto>>();

            var details = await _repository.GetListAsync(p => p.AuthorityId == authrotyId);

            var dtos = new ListResultDto<AuthorityDetailDto>(ObjectMapper.Map<List<AuthorityDetail>, List<AuthorityDetailDto>>(details));

            result.SetData(dtos);
            return result;
        }

        /// <summary>
        /// 获取当前用户的权限对象明细
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("user-details")]
        public async Task<ResultDto<ListResultDto<AuthorityDetailDto>>> LoadUserAll(Guid userId)
        {
            var result = new ResultDto<ListResultDto<AuthorityDetailDto>>();
            var roles = (await _userRoleRepository.GetQueryableAsync()).Where(p => p.UserId == userId);
            var roleAuthoritys = await _roleAuthorityRepository.GetQueryableAsync();
            var aids = (from role in roles
                        join auth in roleAuthoritys
                        on role.RoleId equals auth.RoleId
                        select auth.AuthorityId)
                        .ToList();

            var items = await _repository.GetListAsync(p => aids.Contains(p.AuthorityId));


            var dto = ObjectMapper.Map<List<AuthorityDetail>, List<AuthorityDetailDto>>(items);
            var data = new ListResultDto<AuthorityDetailDto>(dto);

            result.SetData(data);
            return result;

        }
    }
}
