using BaseService.CurrentAuthorityService;
using BaseService.Permissions;
using BaseService.Systems.AuthorityManagerment.Dto;
using Cimc.Helper;
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
using Volo.Abp.ObjectMapping;

namespace BaseService.Systems.AuthorityManagerment
{
    /// <summary>
    /// 权限对象
    /// </summary>
    [Area("Base")]
    [Route("api/base/authority")]
    [Authorize]
    public class AuthorityAppService : ApplicationService, IAuthorityAppService
    {
        private readonly IRepository<Authority, Guid> _repository;
        private readonly IRepository<AuthorityDetail, Guid> _detailRepository;
        private readonly IRepository<AuthorityGroup, Guid> _groupRepository;

        private ICurrentUserAuthorityService CurrentUserAuthority;
        public AuthorityAppService(
            IRepository<Authority, Guid> repository,
            IRepository<AuthorityDetail, Guid> detailRepository,
            IRepository<AuthorityGroup, Guid> groupRepository,
              ICurrentUserAuthorityService currentUserAuthority
            )
        {
            _repository = repository;
            _detailRepository = detailRepository;
            _groupRepository = groupRepository;
            CurrentUserAuthority = currentUserAuthority;
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("create")]
        [Authorize(BaseServicePermissions.Authority.Create)]
        public async Task<ResultDto<Guid>> Create(CreateOrUpdateAuthorityDto input)
        {
            var result = new ResultDto<Guid>();

            //var exist = await _repository.FirstOrDefaultAsync(p => p.DisplayName == input.DisplayName);
            //if (exist != null)
            //{
            //    result.Message = $"权限名称:{input.DisplayName},已存在";
            //    return result;
            //}
            var authCode = StringHelper.RandCode(10); //生成随机值

            var entity = new Authority(
                        GuidGenerator.Create(),
                        CurrentTenant.Id,
                        input.DisplayName,
                        input.AuthType,
                        input.Pid,
                        "",
                        input.Sort,
                        true,
                        input.IsActive,
                        input.Remark,
                        input.GroupId,
                        authCode
               );

            var parent = await _repository.FirstOrDefaultAsync(p => p.Id == input.Pid);
            await ChangeLevelModel(entity, parent);
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
        [Authorize(BaseServicePermissions.Authority.Delete)]
        public async Task<ResultDto<bool>> Delete(List<Guid> ids)
        {
            var result = new ResultDto<bool>();
            try
            {
                foreach (var id in ids)
                {
                    var entity = await _repository.FirstOrDefaultAsync(p => p.Id == id);
                    var sub_ids = (await _repository.GetListAsync(p => p.CascadeId.StartsWith(entity.CascadeId))).Select(p => p.Id);
                    var details = await _detailRepository.GetListAsync(p => sub_ids.Contains(p.AuthorityId.Value));
                    if (details.Any())
                    {
                        result.Message = $"请先删除{entity.DisplayName}所有详情数据,再删除权限对象";
                        return result;
                    }
                    else
                    {
                        await _repository.DeleteAsync(entity.Id);
                        await CurrentUnitOfWork.SaveChangesAsync();
                    }
                }

                //await _repository.DeleteManyAsync(ids);
                //await CurrentUnitOfWork.SaveChangesAsync();

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
        public async Task<ResultDto<AuthorityDto>> Get(Guid id)
        {
            var result = new ResultDto<AuthorityDto>();

            var query = await _repository.GetAsync(id);
            var dto = ObjectMapper.Map<Authority, AuthorityDto>(query);

            result.SetData(dto);
            return result;
        }

        /// <summary>
        /// 分页列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("list")]
        public async Task<ResultDto<PagedResultDto<AuthorityDto>>> GetList(GetAuthorityInputDto input)
        {
            var result = new ResultDto<PagedResultDto<AuthorityDto>>();

            var query = (await _repository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), p => p.DisplayName.Contains(input.Filter))
                .WhereIf(input.AuthType.HasValue, p => p.AuthType == input.AuthType.Value);

            if (input.Id.HasValue)
            {
                var org = await _repository.GetAsync(input.Id.Value);
                query = query.Where(p => p.CascadeId.Contains(org.CascadeId));
            }

            //查询分组
            var groups = await _groupRepository.GetListAsync();

            var items = await query.OrderBy(input.Sorting ?? "Sort")
                     .Skip(input.SkipCount)
                     .Take(input.MaxResultCount)
                     .ToListAsync();

            var totalCount = await query.CountAsync();

            var list = (from item in items
                        join g in groups
                        on item.GroupId equals g.Id
                        into tmp
                        from grp in tmp.DefaultIfEmpty()
                        select new AuthorityDto
                        {
                            Id = item.Id,
                            DisplayName = item.DisplayName,
                            Pid = item.Pid,
                            AuthType = item.AuthType,
                            FullName = item.FullName,
                            Leaf = item.Leaf,
                            CascadeId = item.CascadeId,
                            Remark = item.Remark,
                            IsActive = item.IsActive,
                            Sort = item.Sort,
                            CreationTime = item.CreationTime,
                            GroupId = item.GroupId,
                            GroupName = grp == null ? "" : grp.GroupName
                        }).ToList();

            //var list = ObjectMapper.Map<List<Authority>, List<AuthorityDto>>(items);
            var data = new PagedResultDto<AuthorityDto>(totalCount, list);

            result.SetData(data);
            return result;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("update")]
        [Authorize(BaseServicePermissions.Authority.Update)]
        public async Task<ResultDto<bool>> Update(CreateOrUpdateAuthorityDto input)
        {

            var result = new ResultDto<bool>();

            if (input.Pid == input.Id.Value)
            {
                result.Message = "上级不能为当前名称！";
                return result;
            }

            var query = await _repository.FirstOrDefaultAsync(p => p.Id == input.Id.Value);
            var oldPid = query.Pid;

            #region 处理父节点或名称变更的逻辑，递归处理下级对象 

            if (query.Pid != input.Pid || query.DisplayName != input.DisplayName?.TrimEnd())
            {
                //当前父级
                var parent = await _repository.FirstOrDefaultAsync(p => p.Id == input.Pid);
                query.FullName = (parent == null ? "" : parent.FullName + "/") + input.DisplayName?.TrimEnd();

                var dbQuery = await _repository.GetQueryableAsync();
                //查询自己下级
                var children = await dbQuery.Where(p => p.CascadeId.StartsWith(query.CascadeId)).OrderBy(p => p.CascadeId).ToListAsync();

                if (query.Pid != input.Pid)
                {
                    //当前父级子级最后一级
                    var lastLevel = await dbQuery.Where(p => p.Pid == parent.Id).OrderByDescending(p => p.CascadeId).FirstOrDefaultAsync();
                    var cascadeId = lastLevel == null ? 1 : int.Parse(lastLevel.CascadeId.TrimEnd('.').Split('.').Last()) + 1;
                    query.CascadeId = parent.CascadeId + cascadeId.ToString("D3") + ".";
                }

                if (children.Count > 1)
                {
                    query.Leaf = false;
                    await ChangeChildrenModel(query, children);
                }
                else
                {
                    query.Leaf = true;
                }
            }
            #endregion
            query.Pid = input.Pid;
            query.DisplayName = input.DisplayName?.TrimEnd();
            query.AuthType = input.AuthType;
            query.Sort = input.Sort;
            query.IsActive = input.IsActive;
            query.Remark = input.Remark;
            query.GroupId = input.GroupId;
            await _repository.UpdateAsync(query);

            //更新新旧父节点叶子节点标记
            if (input.Pid != oldPid)
                await ChangeParentLeaf(input.Pid.Value, oldPid.Value);

            result.SetData(true);
            return result;
        }

        /// <summary>
        ///  产品ID查询所有模型服务
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("all")]
        public async Task<ResultDto<ListResultDto<AuthorityDto>>> LoadAll(Guid? id)
        {
            var result = new ResultDto<ListResultDto<AuthorityDto>>();

            var queryable = await _repository.GetQueryableAsync();
            var cid = queryable.Where(p => p.Id == id).Select(p => p.CascadeId).FirstOrDefault();

            //var items = new List<Authority>();
            // var query = id.HasValue ? queryable.Where(p => p.Pid == id) :queryable.Where(p => p.Pid == null);

            var query = id.HasValue ? queryable.Where(p => p.CascadeId.StartsWith(cid)) : queryable.Where(p => p.Pid == null);
            var items = await query.OrderBy(p => p.Sort).ToListAsync();

            var dto = new ListResultDto<AuthorityDto>(ObjectMapper.Map<List<Authority>, List<AuthorityDto>>(items));

            result.SetData(dto);
            return result;
        }

        /// <summary>
        /// 当前用户所拥有的权限对象
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="authType"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("all-nodes")]
        public async Task<ResultDto<ListResultDto<AuthorityDto>>> LoadAllNodes()
        {
            var result = new ResultDto<ListResultDto<AuthorityDto>>();

            //var items = await (await _repository.GetQueryableAsync())
            // .WhereIf(!string.IsNullOrWhiteSpace(filter), p => p.DisplayName.Contains(filter))
            // .WhereIf(authType.HasValue, p => p.AuthType == authType.Value)
            // .OrderBy(p => p.Sort).ToListAsync();


            //var items = new List<Authority>();
            //if (CurrentUser.UserName == BaseConsts.SuperAdmin)
            //{ 
            //    items = await (await _repository.GetQueryableAsync()).WhereIf(!string.IsNullOrWhiteSpace(filter), p => p.DisplayName.Contains(filter)).OrderBy(p => p.Sort).ToListAsync();
            //}
            //else
            //{
            //    //查询当前登录人所属公司的权限对象，不需要显示所有
            //    var roleIds = (await RoleAppService.GetRoles(CurrentUser.Id.Value)).Data.Items.Select(p => p.Id).ToList();//用户绑定的角色 
            //    var roleAuths = await _roleAuthorityRepository.GetListAsync(p => roleIds.Contains(p.RoleId)); //权限对象角色表

            //    var authorityIds = roleAuths.Select(p => p.AuthorityId).ToArray();
            //    var query = await _repository.GetListAsync(p => authorityIds.Contains(p.Id) && p.AuthType >= 2);//权限对象表,只显示到公司（工厂）层级及以下

            //    items = (await UserAuthority(query)).WhereIf(!string.IsNullOrWhiteSpace(filter), p => p.DisplayName.Contains(filter)).OrderBy(p => p.Sort).ToList();

            //}

            var items = await CurrentUserAuthority.GetAuthoritys(null);

            //var dtos = ObjectMapper.Map<List<Authority>, List<AuthorityDto>>(items);

            //查询分组
            var groups = await _groupRepository.GetListAsync();

            var dtos = (from item in items
                        join g in groups
                        on item.GroupId equals g.Id
                        into tmp
                        from grp in tmp.DefaultIfEmpty()
                        select new AuthorityDto
                        {
                            Id = item.Id,
                            DisplayName = item.DisplayName,
                            Pid = item.Pid,
                            AuthType = item.AuthType,
                            FullName = item.FullName,
                            Leaf = item.Leaf,
                            CascadeId = item.CascadeId,
                            Remark = item.Remark,
                            IsActive = item.IsActive,
                            Sort = item.Sort,
                            CreationTime = item.CreationTime,
                            GroupId = item.GroupId,
                            GroupName = grp == null ? "" : grp.GroupName
                        }).ToList();


            var data = new ListResultDto<AuthorityDto>(dtos);

            result.SetData(data);
            return result;
        }

        /// <summary>
        /// 获取当前角色的权限对象 [弃用]
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("role-nodes")]
        public async Task<ResultDto<ListResultDto<AuthorityDto>>> LoadRoleAll(Guid roleId)
        {
            var result = new ResultDto<ListResultDto<AuthorityDto>>();

            var items = (await CurrentUserAuthority.GetAuthoritys(roleId)).OrderBy(p => p.Sort).ToList();

            //var dtos = new ListResultDto<AuthorityDto>(ObjectMapper.Map<List<Authority>, List<AuthorityDto>>(items));

            //查询分组
            var groups = await _groupRepository.GetListAsync();

            var dtos = (from item in items
                        join g in groups
                        on item.GroupId equals g.Id
                        into tmp
                        from grp in tmp.DefaultIfEmpty()
                        select new AuthorityDto
                        {
                            Id = item.Id,
                            DisplayName = item.DisplayName,
                            Pid = item.Pid,
                            AuthType = item.AuthType,
                            FullName = item.FullName,
                            Leaf = item.Leaf,
                            CascadeId = item.CascadeId,
                            Remark = item.Remark,
                            IsActive = item.IsActive,
                            Sort = item.Sort,
                            CreationTime = item.CreationTime,
                            GroupId = item.GroupId,
                            GroupName = grp == null ? "" : grp.GroupName
                        }).ToList();


            var data = new ListResultDto<AuthorityDto>(dtos);

            result.SetData(data);
            return result;
        }

        /// <summary>
        /// 获取当前用户的权限对象(树)
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("role-authority-tree")]
        public async Task<ResultDto<ListResultDto<AuthorityTreeDto>>> GetAuthorityTree(Guid roleId)
        {
            var result = new ResultDto<ListResultDto<AuthorityTreeDto>>();

            var items = (await CurrentUserAuthority.GetAuthorityTree(roleId));

            var dto = new ListResultDto<AuthorityTreeDto>(items);

            result.SetData(dto);
            return result;
        }

        #region 私有方法

        //private async Task<List<Authority>> UserAuthority(List<Authority> list)
        //{
        //    //var roleIds = (await RoleAppService.GetRoles(userId)).Data.Items.Select(p => p.Id).ToList();//用户绑定的角色 
        //    //var roleAuths = await _roleAuthorityRepository.GetListAsync(p => roleIds.Contains(p.RoleId)); //权限对象角色表
        //    //var ids = roleAuths.Select(p => p.AuthorityId).ToArray();
        //    //Guid[] aids = Array.ConvertAll(ids, x => x ?? Guid.NewGuid());
        //    //var query = await _repository.GetListAsync(p => aids.Contains(p.Id) && p.AuthType >= 2); //权限对象表,只显示到公司（工厂）层级及以下


        //    List<string> cascadeIds = new List<string>();
        //    foreach (var auth in list)
        //    {
        //        if (auth.AuthType == 2)
        //        {
        //            cascadeIds.Add(auth.CascadeId);
        //        }
        //        else
        //        {
        //            string[] cascadeArrary = auth.CascadeId.TrimEnd('.').Split('.'); //0.002.002.003
        //            var strCid = $"{cascadeArrary[0]}.{cascadeArrary[1]}.{cascadeArrary[2]}.";
        //            cascadeIds.Add(strCid);
        //        }
        //    }

        //    var items = await _repository.GetQueryableAsync();
        //    var where = PredicateBuilder.New<Authority>();

        //    foreach (var cids in cascadeIds.Distinct().ToList())
        //    {
        //        where = where.Or(p => p.CascadeId.StartsWith(cids));
        //    }

        //    return await items.Where(where).ToListAsync();
        //}

        private async Task ChangeLevelModel(Authority model, Authority parent)
        {
            var cascadeId = model.CascadeId == null ? 1 : int.Parse(model.CascadeId.TrimEnd('.').Split('.').Last());

            if (parent != null && parent.Leaf) parent.Leaf = false;
            var lastLevel = (await _repository.GetQueryableAsync())
                  .Where(p => p.Pid == model.Pid && p.Id != model.Id)
                  .OrderByDescending(p => p.CascadeId)
                  .FirstOrDefault();
            cascadeId = lastLevel == null ? 1 : int.Parse(lastLevel.CascadeId.TrimEnd('.').Split('.').Last()) + 1;

            if (model.Pid.HasValue)
            {
                if (parent != null)
                {
                    //TODO：限制层级数
                    model.CascadeId = parent.CascadeId + cascadeId.ToString("D3") + ".";
                    model.FullName = parent.FullName + "/" + model.DisplayName;
                }
            }
            else
            {
                //TODO：限制层级数
                model.CascadeId = "0." + cascadeId.ToString("D3") + ".";
                model.FullName = model.DisplayName;
            }

        }

        /// <summary>
        /// 更新下级层级关系
        /// </summary>
        /// <param name="root"></param>
        /// <param name="datalist"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        private async Task ChangeChildrenModel(Authority root, List<Authority> datalist, int level = 0)
        {
            level++;
            var children = datalist.Where(p => p.Pid == root.Id).OrderBy(p => p.CascadeId).ToList();
            int count = 0;
            foreach (var item in children)
            {
                count++;
                var model = item;
                model.FullName = root.FullName + "/" + item.DisplayName;
                model.CascadeId = root.CascadeId + count.ToString("D3") + ".";

                var sublist = datalist.Where(p => p.Pid == item.Id).Count();
                if (sublist > 0)
                {
                    model.Leaf = false;
                    await ChangeChildrenModel(model, datalist, level);
                }
                else
                {
                    model.Leaf = true;
                }
                await _repository.UpdateAsync(model);

            }
        }
        /// <summary>
        /// 更新父节点
        /// </summary>
        /// <param name="newPid"></param>
        /// <param name="oldPid"></param>
        /// <returns></returns>
        private async Task ChangeParentLeaf(Guid newPid, Guid oldPid)
        {
            //新的父级
            var new_parent = await _repository.FirstOrDefaultAsync(p => p.Id == newPid);
            new_parent.Leaf = false;
            await _repository.UpdateAsync(new_parent);

            //旧的父级
            var old_list = await _repository.GetListAsync(p => p.Id == oldPid || p.Pid == oldPid);
            var old_parent = old_list.Where(p => p.Id == oldPid).FirstOrDefault();
            if (old_list.Count <= 2)
            {
                old_parent.Leaf = true;
            }
            else
            {
                old_parent.Leaf = false;
            }
            await _repository.UpdateAsync(old_parent);
        }


        #endregion

    }
}
