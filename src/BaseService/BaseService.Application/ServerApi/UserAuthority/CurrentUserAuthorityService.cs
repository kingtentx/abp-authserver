using BaseService.Consts;
using BaseService.Enums;
using BaseService.Systems;
using BaseService.Systems.AuthorityManagerment.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Repositories;

namespace BaseService.CurrentAuthorityService
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [DisableAuditing]
    public class CurrentUserAuthorityService : ApplicationService, ICurrentUserAuthorityService
    {
        private readonly IRepository<Authority, Guid> _repository;
        private readonly IRepository<RoleAuthority> _roleAuthorityRepository;
        private readonly IRepository<UserRole> _userRoleRepository;

        public CurrentUserAuthorityService(
            IRepository<Authority, Guid> repository,
            IRepository<RoleAuthority> roleAuthorityRepository,
            IRepository<UserRole> userRoleRepository
           )
        {
            _repository = repository;
            _roleAuthorityRepository = roleAuthorityRepository;
            _userRoleRepository = userRoleRepository;

        }

        /// <summary>
        /// 查前当前登录用户的权限对象
        /// </summary>
        /// <param name="authType"> </param>
        /// <returns></returns>
        public async Task<List<AuthorityDto>> GetAuthoritys(int? authType)
        {
            List<Authority> list = new List<Authority>();

            if (CurrentUser.UserName.ToLower().Equals(SystemConsts.SuperAdmin))//超级管理员直接获取所有权限对象
            {
                list = await (await _repository.GetQueryableAsync()).WhereIf(authType.HasValue, p => p.AuthType == authType.Value).OrderBy(p => p.Sort).ToListAsync();
            }
            else
            {
                var user_role = await _userRoleRepository.GetQueryableAsync();
                var role_auth = await _roleAuthorityRepository.GetQueryableAsync();

                var authorityIds = await (from role in user_role
                                          join auth in role_auth
                                          on role.RoleId equals auth.RoleId
                                          where role.UserId == CurrentUser.Id
                                          select auth.AuthorityId)
                                        .ToListAsync();

                var userAuths = await (await _repository.GetQueryableAsync())
                    .Where(p => authorityIds.Contains(p.Id))
                    .WhereIf(authType.HasValue, p => p.AuthType == authType.Value)
                    .ToListAsync();

                list = (await this.ProcessAuthority(userAuths)).WhereIf(authType.HasValue, p => p.AuthType == authType.Value).OrderBy(p => p.Sort).ToList();
            }
            var dtos = ObjectMapper.Map<List<Authority>, List<AuthorityDto>>(list);
            return dtos;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task<List<AuthorityDto>> GetAuthoritys(Guid roleId)
        {
            var treelist = new List<AuthorityTreeDto>();

            var roleAuth = await _roleAuthorityRepository.GetQueryableAsync();

            var auths = (from aid in roleAuth
                         join a in (await _repository.GetQueryableAsync()) on aid.AuthorityId equals a.Id
                         where aid.RoleId == roleId
                         select a).ToList();

            var list = await ProcessAuthority(auths);
            var dtos = ObjectMapper.Map<List<Authority>, List<AuthorityDto>>(list);
            return dtos;
        }

        private async Task<List<Authority>> ProcessAuthority(List<Authority> list)
        {
            List<string> cascadeIds = new List<string>();
            foreach (var auth in list)
            {
                if (auth.AuthType == (int)AuthType.Company) //0.001.
                {
                    cascadeIds.Add(auth.CascadeId);
                }
                else
                {
                    string[] cascadeArrary = auth.CascadeId.TrimEnd('.').Split('.'); //0.001.001.
                    var strCid = $"{cascadeArrary[0]}.{cascadeArrary[1]}.";
                    cascadeIds.Add(strCid);
                }
            }

            var items = await _repository.GetQueryableAsync();
            var where = PredicateBuilder.New<Authority>();

            foreach (var cids in cascadeIds.Distinct().ToList())
            {
                where = where.Or(p => p.CascadeId.StartsWith(cids));
            }

            return await items.Where(where).ToListAsync();
        }


        public async Task<List<AuthorityTreeDto>> GetAuthorityTree(Guid roleId)
        {
            var treelist = new List<AuthorityTreeDto>();

            var roleAuth = await _roleAuthorityRepository.GetQueryableAsync();

            var auths = (from aid in roleAuth
                         join a in (await _repository.GetQueryableAsync()) on aid.AuthorityId equals a.Id
                         where aid.RoleId == roleId
                         select a).ToList();


            var datalist = await GetAuthoritys(null);
            var roots = datalist.Where(p => p.AuthType == (int)AuthType.Company).ToList();
            //处理为树结构
            treelist = LoadAuthorityTree(roots, datalist, auths);

            return treelist;
        }

        private List<AuthorityTreeDto> LoadAuthorityTree(List<AuthorityDto> roots, List<AuthorityDto> datalist, List<Authority> roleAuthoritys)
        {
            var result = new List<AuthorityTreeDto>();
            foreach (var root in roots.OrderBy(p => p.Sort))
            {
                var tree = new AuthorityTreeDto
                {
                    Id = root.Id,
                    Name = root.DisplayName,
                    Pid = root.Pid,
                    AuthType = root.AuthType,
                    Sort = root.Sort,
                    IsChecked = roleAuthoritys.Where(p => p.Id == root.Id).Count() > 0
                };
                if (datalist.Where(p => p.Pid == root.Id).Any())
                {
                    tree.Children = LoadAuthorityTree(datalist.Where(p => p.Pid == root.Id).ToList(), datalist, roleAuthoritys);
                }
                else
                {
                    tree.Children = new List<AuthorityTreeDto>();
                }
                result.Add(tree);
            }
            return result;
        }
    }
}
