using BaseService.Consts;
using BaseService.Enums;
using BaseService.Systems.AuthorityManagerment.Dto;
using BaseService.Systems.MenuManagement.Dto;
using BaseService.Systems.RoleMenusManagement;
using BaseService.Systems.RoleMenusManagement.Dto;
using Cimc.Model.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.PermissionManagement;

namespace BaseService.Systems.UserMenusManagement
{
    /// <summary>
    /// 角色菜单/权限
    /// </summary> 
    [Area("Base")]
    [Route("api/base/role-menu")]
    [Authorize]
    public class RoleMenusAppService : ApplicationService, IRoleMenusAppService
    {      
        private readonly IRepository<Menu, Guid> _menuRepository;
        private readonly IRepository<RoleMenu> _roleMenuRepository;
        private readonly IRepository<RoleAuthority> _roleAuthorityRepository;
        private readonly IRepository<User, Guid> _userRepository;
        private readonly IRepository<UserRole> _userRoleRepository;
        private readonly IRepository<Authority, Guid> _authorityRepository;
        private readonly IRepository<Role, Guid> _roleRepository;
        private readonly IPermissionAppService PermissionAppService;

        private List<Menu> tenantMenus = new List<Menu>();

        public RoleMenusAppService(          
            IRepository<Menu, Guid> menuRepository,
            IRepository<RoleMenu> roleMenuRepository,
            IRepository<RoleAuthority> roleAuthorityRepository,
            IRepository<User, Guid> userRepository,
            IRepository<UserRole> userRoleRepository,
            IRepository<Authority, Guid> authorityRepository,
            IRepository<Role, Guid> roleRepository,
            IPermissionAppService permissionAppService

            )
        {
            _roleRepository = roleRepository;
            _menuRepository = menuRepository;
            _roleMenuRepository = roleMenuRepository;
            _roleAuthorityRepository = roleAuthorityRepository;
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _authorityRepository = authorityRepository;

            PermissionAppService = permissionAppService;
        }

        /// <summary>
        /// 更新角色菜单
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("update")]
        [Authorize(IdentityPermissions.Roles.ManagePermissions)]
        public async Task<ResultDto<bool>> Update(UpdateRoleMenuDto input)
        {
            var result = new ResultDto<bool>();
            try
            {
                var roleMenus = new List<RoleMenu>();
                foreach (var menuId in input.MenuIds)
                {
                    roleMenus.Add(new RoleMenu(CurrentTenant.Id, input.RoleId, menuId));
                }
                var roleAuthoritys = new List<RoleAuthority>();
                foreach (var authId in input.AuthorityIds)
                {
                    roleAuthoritys.Add(new RoleAuthority(CurrentTenant.Id, input.RoleId, authId));
                }

                var role = await _roleRepository.GetAsync(input.RoleId);

                #region  处理权限

                string providerName = ProviderNameType.R.ToString(); //R-角 U-用户
                string providerKey = role.Name; //角色名称

                var menuList = await _menuRepository.GetListAsync(p => input.MenuIds.Contains(p.Id));
                List<UpdatePermissionDto> permissions = new();
                foreach (var menu in menuList)
                {
                    if (!string.IsNullOrWhiteSpace(menu.Auths))
                    {
                        permissions.Add(
                            new UpdatePermissionDto()
                            {
                                Name = menu.Auths,
                                IsGranted = true
                            });
                    }
                }

                var updateDto = new UpdatePermissionsDto()
                {
                    Permissions = permissions.DistinctBy(p => p.Name).ToArray() //权限名称重复的去重，否则更新异常
                };

                if (updateDto.Permissions.Any())
                    await PermissionAppService.UpdateAsync(providerName, providerKey, updateDto);//注意此操作需要权限:AbpIdentity.Roles.ManagePermissions

                #endregion

                #region 更新角色菜单          

                //删除角色旧菜单
                var old_list = await _menuRepository.GetListAsync(p => p.ClientType == input.ClientType);
                await _roleMenuRepository.DeleteAsync(p => old_list.Select(p => p.Id).Contains(p.MenuId) && p.RoleId == role.Id);
                //插入角色新菜单
                await _roleMenuRepository.InsertManyAsync(roleMenus);
                #endregion

                //更新权限对象
                await _roleAuthorityRepository.DeleteAsync(p => p.RoleId == input.RoleId);
                await _roleAuthorityRepository.InsertManyAsync(roleAuthoritys);

                await CurrentUnitOfWork.SaveChangesAsync();

                result.SetData(true);
            }
            catch (Exception ex)
            {
                var msg = "更新角色权限错误" + ex.Message;
                Log.Error(msg);
                result.Code = (int)ResultCode.ServerError;
                result.Message = msg;
            }
            return result;
        }


        /// <summary>
        /// 获取角色菜单（树结构）
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("tree")]
        public async Task<ResultDto<ListResultDto<MenusTreeDto>>> GetUserMenusTree(int? clientType)
        {
            var result = new ResultDto<ListResultDto<MenusTreeDto>>();

            List<Menu> menus = new List<Menu>();
            if (CurrentUser.UserName.ToLower().Equals(SystemConsts.SuperAdmin) && CurrentUser.TenantId == null)//平台超级管理员直接获取所有菜单
            {
                menus = await _menuRepository.GetListAsync(p => p.ClientType == clientType.Value);
            }
            else
            {
                var userRoles = await _userRoleRepository.GetQueryableAsync();
                var roleMenuss = await _roleMenuRepository.GetQueryableAsync();

                var menuIds = await (from ur in userRoles
                                     join rm in roleMenuss
                                   on ur.RoleId equals rm.RoleId
                                     where ur.UserId == CurrentUser.Id
                                     select rm.MenuId).ToListAsync();

                menus = await _menuRepository.GetListAsync(p => menuIds.Contains(p.Id) && p.ClientType == clientType.Value);
            }

            var dtos = ObjectMapper.Map<List<Menu>, List<MenusTreeDto>>(menus);
            var data = new ListResultDto<MenusTreeDto>(dtos.OrderBy(p => p.Sort).ToList());

            #region 作废
            //List<Menu> menus = new List<Menu>();
            //if (CurrentUser.UserName.ToLower().Equals(BaseConsts.SuperAdmin))//超级管理员直接获取所有菜单
            //{
            //    menus = await _menuRepository.GetListAsync();
            //}
            //else
            //{
            //    var userRoles = await _userRoleRepository.GetQueryableAsync();
            //    var roleMenuss = await _roleMenuRepository.GetQueryableAsync();

            //    var menuIds = await (from ur in userRoles
            //                         join rm in roleMenuss
            //                       on ur.RoleId equals rm.RoleId
            //                         where ur.UserId == CurrentUser.Id
            //                         select rm.MenuId).ToListAsync();

            //    menus = await _menuRepository.GetListAsync(p => menuIds.Contains(p.Id));
            //}

            //var dtos = ObjectMapper.Map<List<Menu>, List<MenusTreeDto>>(menus);
            //var data = new ListResultDto<MenusTreeDto>(dtos.OrderBy(p => p.Sort).ToList());
            #endregion

            result.SetData(data);
            return result;
        }

        ///// <summary>
        ///// 获取角色所有菜单
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("menus")]
        //public async Task<ResultDto<ListResultDto<MenuNodesDto>>> GetRoleMenus()
        //{
        //    var result = new ResultDto<ListResultDto<MenuNodesDto>>();

        //    var roleIds = await (await _roleRepository.GetQueryableAsync()).Where(p => CurrentUser.Roles.Contains(p.Name)).Select(p => p.Id).ToListAsync();
        //    // var roleMenus = await (await _roleMenuRepository.GetQueryableAsync()).Where(p => roleIds.Contains(p.RoleId)).Select(p => p.MenuId).ToListAsync();
        //    //var menus = await _menuRepository.GetListAsync(p => p.CategoryId == 1 && roleMenus.Contains(p.Id));
        //    List<Guid> roleMenus = new List<Guid>();
        //    List<Menu> menus = new List<Menu>();

        //    if (CurrentUser.UserName.ToLower().Equals(SystemConsts.SuperAdmin))//超级管理员直接获取所有菜单
        //    {
        //        //roleMenus = await (await _menuRepository.GetQueryableAsync()).Select(p => p.Id).ToListAsync();
        //        menus = await _menuRepository.GetListAsync(p => p.MenuType <= (int)MenuType.Menu);
        //    }
        //    else
        //    {
        //        roleMenus = await (await _roleMenuRepository.GetQueryableAsync()).Where(p => roleIds.Contains(p.RoleId)).Select(p => p.MenuId).ToListAsync();
        //        menus = await _menuRepository.GetListAsync(p => p.MenuType <= (int)MenuType.Menu && roleMenus.Contains(p.Id));
        //    }

        //    var root = menus.Where(p => p.ParentId == null).OrderBy(p => p.Sort).ToList();
        //    var data = new ListResultDto<MenuNodesDto>(LoadRoleMenusTree(root, menus));

        //    result.SetData(data);
        //    return result;
        //}

        /// <summary>
        /// 获取角色菜单ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="clientType"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<ResultDto<ListResultDto<Guid>>> GetRoleMenuIds(Guid id, int clientType = 0)
        {
            var result = new ResultDto<ListResultDto<Guid>>();
            var menus = await _menuRepository.GetQueryableAsync();
            var role_menus = await _roleMenuRepository.GetQueryableAsync();

            var menuIds = (from m in menus
                           join rm in role_menus
                           on m.Id equals rm.MenuId
                           where m.ClientType == clientType && rm.RoleId == id && rm.TenantId == CurrentTenant.Id
                           select m.Id)
                           .ToList();

            var data = new ListResultDto<Guid>(menuIds);

            result.SetData(data);
            return result;
        }

        //private List<MenuNodesDto> LoadRoleMenusTree(List<Menu> roots, List<Menu> menus)
        //{
        //    var result = new List<MenuNodesDto>();
        //    foreach (var root in roots)
        //    {
        //        var menu = new MenuNodesDto
        //        {
        //            Path = root.Path,                 
        //            MenuType = root.MenuType,
        //            ParentId = root.ParentId,
        //            HigherMenuOptions = root.HigherMenuOptions,
        //            Title = root.Title?.Trim(),
        //            Route = root.Route?.Trim(),
        //            Component = root.Component,
        //            Sort = root.Sort,
        //            Redirect = root.Redirect?.Trim(),
        //            Icon = root.Icon,
        //            ExtraIcon = root.ExtraIcon,
        //            EnterTransition = root.EnterTransition,
        //            LeaveTransition = root.LeaveTransition,
        //            ActivePath = root.ActivePath,
        //            Auths = root.Auths?.Trim(),
        //            FrameSrc = root.FrameSrc?.Trim(),
        //            FrameLoading = root.FrameLoading?.Trim(),
        //            KeepAlive = root.KeepAlive,
        //            HiddenTag = root.HiddenTag,
        //            FixedTag = root.FixedTag,
        //            ShowLink = root.ShowLink,
        //            ShowParent = root.ShowParent
        //        };
        //        if (menus.Where(p => p.ParentId == root.Id).Any())
        //        {
        //            menu.Children = LoadRoleMenusTree(menus.Where(p => p.ParentId == root.Id).OrderBy(p => p.Sort).ToList(), menus);
        //        }
        //        result.Add(menu);
        //    }
        //    return result;
        //}

        #region 权限对象

        /// <summary>
        ///  获取角色所有权限对象
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("authoritys")]
        public async Task<ResultDto<ListResultDto<AuthorityDto>>> GetRoleAuthoritys()
        {
            var result = new ResultDto<ListResultDto<AuthorityDto>>();

            //var roleIds = await (await _userRoleRepository.GetQueryableAsync()).Where(p => p.RoleId.Equals(CurrentUser.Id)).Select(p => p.RoleId).ToListAsync();

            //var authorityIds = await (await _roleAuthorityRepository.GetQueryableAsync()).Where(p => roleIds.Contains(p.AuthorityId)).Select(p => p.AuthorityId).ToListAsync();

            var roleIds = (await _userRoleRepository.GetQueryableAsync()).Where(p => p.RoleId.Equals(CurrentUser.Id)).Select(p => p.RoleId);
            var roleAuthoritys = await _roleAuthorityRepository.GetQueryableAsync();

            var authorityIds = await (from rids in roleIds
                                      join auths in roleAuthoritys
                                      on rids equals auths.AuthorityId
                                      select new List<Guid> { auths.AuthorityId.Value }).FirstOrDefaultAsync();

            var authoritys = await _authorityRepository.GetListAsync(p => authorityIds.Contains(p.Id));

            var data = new ListResultDto<AuthorityDto>(ObjectMapper.Map<List<Authority>, List<AuthorityDto>>(authoritys));

            result.SetData(data);
            return result;


        }

        #endregion


        #region 修改租户菜单方法
        /// <summary>
        /// 更新租户菜单
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("update-tenant")]
        [Authorize(IdentityPermissions.Roles.Update)]
        public async Task<ResultDto<bool>> UpdateTenantMenus(UpdateTenantMenuDto input)
        {
            var result = new ResultDto<bool>();

            using (CurrentTenant.Change(input.TenantId))
            {
                //当前租户管理员角色
                var role = await _roleRepository.GetAsync(p => p.IsStatic == true);

                //创建当前租户管理员角色菜单
                var roleMenus = new List<RoleMenu>();
                foreach (var menuId in input.MenuIds)
                {
                    roleMenus.Add(new RoleMenu(input.TenantId, role.Id, menuId));
                }

                //删除角色旧菜单
                var old_list = await _menuRepository.GetListAsync(p => p.ClientType == input.ClientType);
                await _roleMenuRepository.DeleteAsync(p => old_list.Select(p => p.Id).Contains(p.MenuId) && p.RoleId == role.Id && p.TenantId == input.TenantId);
                //插入角色新菜单
                await _roleMenuRepository.InsertManyAsync(roleMenus);

                await CurrentUnitOfWork.SaveChangesAsync();
            }

            result.SetData(true);
            return result;
        }


        /// <summary>
        /// 获取租户的菜单id集合
        /// </summary>
        /// <param name="tenantId">租户ID</param>
        /// <param name="clientType">客户端类型 0-PC,1-APP,2-H5,5-外链</param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-tenant")]
        public async Task<ResultDto<ListResultDto<Guid>>> GetTenantMenuIds(Guid tenantId, int clientType)
        {
            var result = new ResultDto<ListResultDto<Guid>>();

            using (CurrentTenant.Change(tenantId))
            {
                var role_menuIds = (from r in await _roleRepository.GetQueryableAsync()
                                    join rm in await _roleMenuRepository.GetQueryableAsync()
                                    on r.Id equals rm.RoleId
                                    where r.IsStatic == true && r.TenantId == CurrentTenant.Id
                                    select rm.MenuId).ToList();

                var menus = await _menuRepository.GetListAsync(p => role_menuIds.Contains(p.Id) && p.ClientType == clientType);

                var data = new ListResultDto<Guid>(menus.Select(p => p.Id).ToList());
                result.SetData(data);
            }
            return result;
        }

        #endregion
    }
}
