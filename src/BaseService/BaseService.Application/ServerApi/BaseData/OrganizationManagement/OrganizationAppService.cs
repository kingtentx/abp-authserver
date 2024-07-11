using BaseService.BaseData.OrganizationManagement.Dto;
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
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;

namespace BaseService.BaseData.OrganizationManagement
{
    [Area("Base")]
    [Route("api/base/orgs")]
    [Authorize]
    public class OrganizationAppService : BaseApplicationService, IOrganizationAppService
    {
        private readonly IRepository<Organization, Guid> _repository;
        private readonly IRepository<User, Guid> _userRepository;
        private readonly IRepository<UserOrganization> _userOrgRepository;

        public OrganizationAppService(
            IRepository<Organization, Guid> repository,
            IRepository<User, Guid> userRepository,
            IRepository<UserOrganization> userOrgRepository,
            IDefaultAppService defaultAppService
            ) : base(defaultAppService)
        {
            _repository = repository;
            _userRepository = userRepository;
            _userOrgRepository = userOrgRepository;
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("create")]
        [Authorize(BaseServicePermissions.Organization.Create)]
        public async Task<ResultDto<Guid>> Create(CreateOrUpdateOrganizationDto input)
        {
            var result = new ResultDto<Guid>();
            var authorityId = CurrentAuthority.Id;

            var exist = await _repository.FirstOrDefaultAsync(p => p.Name == input.Name && p.AuthorityId == authorityId);
            if (exist != null)
            {
                //throw new BusinessException("名称：" + input.Name + "机构已存在");
                result.Message = "名称：" + input.Name + "机构已存在";
                return result;
            }


            var organization = new Organization(GuidGenerator.Create(),
                                                CurrentTenant.Id,
                                                input.OrgType,
                                                input.Pid,
                                                input.Name,
                                                "",
                                                input.Sort,
                                                true,
                                                input.IsActive,
                                                authorityId
                                                );
            var parent = await _repository.FirstOrDefaultAsync(p => p.Id == input.Pid);
            await ChangeOrganizationModel(organization, parent);
            var query = await _repository.InsertAsync(organization);

            //var dto = ObjectMapper.Map<Organization, OrganizationDto>(organization);

            result.SetData(query.Id);
            return result;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("update")]
        [Authorize(BaseServicePermissions.Organization.Update)]
        public async Task<ResultDto<bool>> Update(CreateOrUpdateOrganizationDto input)
        {
            var result = new ResultDto<bool>();

            if (input.Pid == input.Id)
            {
                //throw new BusinessException("机构上级不能为当前机构！");
                result.Message = "机构上级不能为当前机构！";
                return result;
            }
            var authorityId = CurrentAuthority.Id;

            var organization = await _repository.FirstOrDefaultAsync(p => p.Id == input.Id.Value);

            //if (organization.Pid != input.Pid)
            //{
            //    var parent = await _repository.FirstOrDefaultAsync(p => p.Id == input.Pid);
            //    var orgs = await (await _repository.GetQueryableAsync()).Where(p => p.CascadeId.Contains(organization.CascadeId) && p.Id != organization.Id && p.AuthorityId == authorityId)
            //                          .OrderBy(p => p.CascadeId).ToListAsync();
            //    organization.Pid = input.Pid;
            //    await ChangeOrganizationModel(organization, parent);
            //    foreach (var org in orgs)
            //    {
            //        if (org.Pid == organization.Id)
            //        {
            //            await ChangeOrganizationModel(org, organization, false);
            //        }
            //        else
            //        {
            //            var orgParent = orgs.FirstOrDefault(p => p.Id == org.Pid);
            //            await ChangeOrganizationModel(org, orgParent, false);
            //        }
            //    }
            //}

            var query = await _repository.FirstOrDefaultAsync(p => p.Id == input.Id.Value);
            var oldPid = query.Pid;

            #region 处理父节点或名称变更的逻辑，递归处理下级对象 

            if (query.Pid != input.Pid || query.Name != input.Name?.TrimEnd())
            {
                //当前父级
                var parent = await _repository.FirstOrDefaultAsync(p => p.Id == input.Pid);
                query.FullName = parent.FullName + "/" + input.Name?.TrimEnd();

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

            organization.Name = input.Name;
            organization.Sort = input.Sort;
            organization.IsActive = input.IsActive;
            organization.OrgType = input.OrgType;

            await _repository.UpdateAsync(organization);
            //var dto = ObjectMapper.Map<Organization, OrganizationDto>(organization);
            //更新新旧父节点叶子节点标记
            if (input.Pid != oldPid)
                await ChangeParentLeaf(input.Pid.Value, oldPid.Value);

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
        [Authorize(BaseServicePermissions.Organization.Delete)]
        public async Task<ResultDto<bool>> Delete(List<Guid> ids)
        {
            var result = new ResultDto<bool>();
            var authorityId = CurrentAuthority.Id;
            try
            {
                foreach (var id in ids)
                {
                    var org = await _repository.GetAsync(id);
                    await _repository.DeleteAsync(p => p.CascadeId.Contains(org.CascadeId) && p.AuthorityId == authorityId);
                }
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
        public async Task<ResultDto<OrganizationDto>> Get(Guid id)
        {
            var result = new ResultDto<OrganizationDto>();
            var query = await _repository.GetAsync(id);

            var dto = ObjectMapper.Map<Organization, OrganizationDto>(query);

            result.SetData(dto);
            return result;
        }

        [HttpGet]
        [Route("list")]
        public async Task<ResultDto<PagedResultDto<OrganizationDto>>> GetList(GetOrganizationInputDto input)
        {
            var result = new ResultDto<PagedResultDto<OrganizationDto>>();
            var authorityId = CurrentAuthority.Id;
            //var aaa = CurrentUser.FindClaim("auth_ids");
            var query = (await _repository.GetQueryableAsync())
                .Where(p => p.AuthorityId == authorityId)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), p => p.Name.Contains(input.Filter))
                .WhereIf(input.OrgType.HasValue, p => p.OrgType == input.OrgType.Value);
            if (input.Id.HasValue)
            {
                var org = await _repository.GetAsync(input.Id.Value);
                query = query.Where(p => p.CascadeId.Contains(org.CascadeId));
            }

            var items = await query.OrderBy(input.Sorting ?? nameof(Organization.Sort))
                     .Skip(input.SkipCount)
                     .Take(input.MaxResultCount)
                     .ToListAsync();
            var totalCount = await query.CountAsync();

            var list = ObjectMapper.Map<List<Organization>, List<OrganizationDto>>(items);
            var data = new PagedResultDto<OrganizationDto>(totalCount, list);

            result.SetData(data);
            return result;
        }

        [HttpGet]
        [Route("load-orgs")]
        public async Task<ResultDto<ListResultDto<OrganizationDto>>> LoadAll(Guid? id, string filter)
        {
            var result = new ResultDto<ListResultDto<OrganizationDto>>();
            var authorityId = CurrentAuthority.Id;

            var queryable = await _repository.GetQueryableAsync();
            var cid = queryable.Where(p => p.Id == id && p.AuthorityId == authorityId).Select(p => p.CascadeId).FirstOrDefault();

            var items = new List<Organization>();
            if (!string.IsNullOrWhiteSpace(filter))
            {
                items = await queryable.Where(p => p.Name.Contains(filter)).OrderBy(p => p.Sort).ToListAsync();
            }
            else
            {
                // var query = id.HasValue ? queryable.Where(p => p.Pid == id) :queryable.Where(p => p.Pid == null);
                var query = id.HasValue ? queryable.Where(p => p.CascadeId.StartsWith(cid)) : queryable.Where(p => p.Pid == null);
                items = await query.OrderBy(p => p.Sort).ToListAsync();
            }

            var dto = new ListResultDto<OrganizationDto>(ObjectMapper.Map<List<Organization>, List<OrganizationDto>>(items));

            result.SetData(dto);
            return result;
        }

        [HttpGet]
        [Route("load-nodes")]
        public async Task<ResultDto<ListResultDto<OrganizationDto>>> LoadAllNodes(string filter)
        {
            var result = new ResultDto<ListResultDto<OrganizationDto>>();

            //List<Organization> items = new List<Organization>();
            //if (CurrentUser.UserName == BaseConsts.SuperAdmin)
            //{
            //    items = await (await _repository.GetQueryableAsync()).WhereIf(!string.IsNullOrWhiteSpace(filter), p => p.Name.Contains(filter)).OrderBy(p => p.Sort).ToListAsync();
            //}
            //else
            //{
            //    //查询当前登录人所属公司的机构，不需要显示所有机构
            //    items = (await UserOrganizations(CurrentUser.Id.Value)).WhereIf(!string.IsNullOrWhiteSpace(filter), p => p.Name.Contains(filter)).OrderBy(p => p.Sort).ToList();
            //}

            var items = await (await _repository.GetQueryableAsync())
                    .Where(p => p.AuthorityId == CurrentAuthority.Id)
                    .WhereIf(!string.IsNullOrWhiteSpace(filter), p => p.Name.Contains(filter))
                    .OrderBy(p => p.Sort)
                    .ToListAsync();

            var dto = ObjectMapper.Map<List<Organization>, List<OrganizationDto>>(items);

            var data = new ListResultDto<OrganizationDto>(dto);

            result.SetData(data);
            return result;
        }

        private async Task<List<Organization>> UserOrganizations(Guid userId)
        {
            var userOrg = await _userOrgRepository.GetQueryableAsync();
            var org = await _repository.GetQueryableAsync();

            var orgInfo = await (from u in userOrg
                                 join o in org
                               on u.OrganizationId equals o.Id
                                 where u.UserId == userId && o.IsDeleted == false
                                 select o).FirstOrDefaultAsync();

            List<string> cascadeIds = new List<string>();

            string[] cascadeArrary = orgInfo.CascadeId.TrimEnd('.').Split('.'); //0.002.002.

            string path = "0.";
            foreach (var item in cascadeArrary)
            {
                if (item != "0")
                {
                    path += item + ".";
                    cascadeIds.Add(path);
                }
            }
            var items = await _repository.GetListAsync(p => cascadeIds.Contains(p.CascadeId) && p.OrgType >= 2);//只显示到公司（工厂）层级及以下

            return items;
        }


        private async Task ChangeOrganizationModel(Organization org, Organization parent)
        {
            var cascadeId = org.CascadeId == null ? 1 : int.Parse(org.CascadeId.TrimEnd('.').Split('.').Last());

            if (parent != null && parent.Leaf) parent.Leaf = false;
            var lastLevel = (await _repository.GetQueryableAsync()).Where(p => p.Pid == org.Pid && p.Id != org.Id)
                  .OrderByDescending(p => p.CascadeId)
                  .FirstOrDefault();
            cascadeId = lastLevel == null ? 1 : int.Parse(lastLevel.CascadeId.TrimEnd('.').Split('.').Last()) + 1;


            if (org.Pid.HasValue)
            {
                //if (parent == null)
                //{
                //    throw new BusinessException("上级机构查询错误！");
                //}
                if (parent != null)
                {
                    //TODO：限制层级数
                    org.CascadeId = parent.CascadeId + cascadeId.ToString("D3") + ".";
                    org.FullName = parent.FullName + "/" + org.Name;
                }
            }
            else
            {
                //TODO：限制层级数
                org.CascadeId = "0." + cascadeId.ToString("D3") + ".";
                org.FullName = org.Name;
            }

        }

        /// <summary>
        /// 更新下级层级关系
        /// </summary>
        /// <param name="root"></param>
        /// <param name="datalist"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        private async Task ChangeChildrenModel(Organization root, List<Organization> datalist, int level = 0)
        {
            level++;
            var children = datalist.Where(p => p.Pid == root.Id).OrderBy(p => p.CascadeId).ToList();
            int count = 0;
            foreach (var item in children)
            {
                count++;
                var model = item;
                model.FullName = root.FullName + "/" + item.Name;
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
    }
}
