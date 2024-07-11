using BaseService.Permissions;
using BaseService.Systems.MenuManagement.Dto;
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

namespace BaseService.Systems.MenuManagement
{
    /// <summary>
    /// 菜单
    /// </summary>
    [Area("Base")]
    [Route("api/base/menu")]
    [Authorize(BaseServicePermissions.Menu.Default)]
    public class MenuAppService : ApplicationService, IMenuAppService
    {
        private readonly IRepository<Menu, Guid> _repository;

        public MenuAppService(IRepository<Menu, Guid> repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// 创建菜单
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("create")]
        public async Task<ResultDto<Guid>> Create(CreateOrUpdateMenuDto input)
        {
            var result = new ResultDto<Guid>();

            var query = await _repository.FindAsync(p => p.Label.Equals(input.Label.Trim()));
            if (query != null)
            {
                result.Message = $"[{input.Label}] 菜单代码已存在。";
                return result;
            }

            var menu = await _repository.InsertAsync(new Menu(GuidGenerator.Create())
            {
                FormId = input.FormId,
                Pid = input.Pid,
                Name = input.Name?.Trim(),
                Label = input.Label?.Trim(),
                CategoryId = input.CategoryId,
                Sort = input.Sort,
                Path = input.Path?.Trim(),
                Component = input.Component?.Trim(),
                Permission = input.Permission?.Trim(),
                Icon = input.Icon,
                AlwaysShow = input.AlwaysShow,
                Hidden = input.Hidden,
                ClientType = input.ClientType,
                Business = input.Business,
                OtherPlatformCode = input.OtherPlatformCode
            });

            result.SetData(menu.Id);
            return result;
        }

        /// <summary>
        /// 更新菜单
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("update")]
        public async Task<ResultDto<bool>> Update(CreateOrUpdateMenuDto input)
        {
            var result = new ResultDto<bool>();

            var query = await _repository.FindAsync(p => p.Label.Equals(input.Label.Trim()) && p.Id != input.Id);
            if (query != null)
            {
                result.Message = $"[{input.Label}] 菜单代码已存在。";
                return result;
            }

            var menu = await _repository.GetAsync(input.Id.Value);
            menu.Pid = input.Pid;
            menu.CategoryId = input.CategoryId;
            menu.Name = input.Name?.Trim();
            menu.Label = input.Label?.Trim();
            menu.Sort = input.Sort;
            menu.Path = input.Path?.Trim();
            menu.Component = input.Component?.Trim();
            menu.Permission = input.Permission?.Trim();
            menu.Icon = input.Icon;
            menu.AlwaysShow = input.AlwaysShow;
            menu.Hidden = input.Hidden;          
            menu.ClientType = input.ClientType;
            menu.Business = input.Business?.Trim();
            menu.OtherPlatformCode = input.OtherPlatformCode?.Trim();

            result.SetData(true);
            return result;
        }

        /// <summary>
        /// 删除菜单
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
        ///  查询分页列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("list")]
        public async Task<ResultDto<ListResultDto<MenuDto>>> GetList(GetMenuInputDto input)
        {
            var result = new ResultDto<ListResultDto<MenuDto>>();

            var query = (await _repository.GetQueryableAsync()).WhereIf(!string.IsNullOrWhiteSpace(input.Filter), _ => _.Name.Contains(input.Filter));
            var items = await query.OrderBy(input.Sorting ?? "Sort")
                                 .Skip(input.SkipCount)
                                 .Take(input.MaxResultCount)
                                 .ToListAsync();
            var dtos = ObjectMapper.Map<List<Menu>, List<MenuDto>>(items);
            var data = new ListResultDto<MenuDto>(dtos);
            result.SetData(data);
            return result;
        }

        /// <summary>
        ///  查询单个实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<ResultDto<MenuDto>> Get(Guid id)
        {
            var result = new ResultDto<MenuDto>();

            var query = await _repository.GetAsync(id);
            var dto = ObjectMapper.Map<Menu, MenuDto>(query);
            if (dto.Pid.HasValue)
                dto.ParentLabel = (await _repository.FirstOrDefaultAsync(_ => _.Id == query.Pid))?.Label;

            result.SetData(dto);
            return result;
        }

        /// <summary>
        /// 查询全部菜单（树）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("load-menus")]
        public async Task<ResultDto<ListResultDto<MenuNodesDto>>> LoadAll(int clientType = 0)
        {
            var result = new ResultDto<ListResultDto<MenuNodesDto>>();

            if (CurrentUser.TenantId == null)
            {
                var menus = await _repository.GetListAsync(p => p.ClientType == clientType);
                var root = menus.Where(_ => _.Pid == null).OrderBy(_ => _.Sort).ToList();
                var data = new ListResultDto<MenuNodesDto>(LoadMenusTree(root, menus));
                result.SetData(data);
            }

            return result;

        }


        private List<MenuNodesDto> LoadMenusTree(List<Menu> roots, List<Menu> menus)
        {
            var result = new List<MenuNodesDto>();
            foreach (var root in roots)
            {
                var menu = new MenuNodesDto
                {
                    Id = root.Id,
                    Pid = root.Pid,
                    CategoryId = root.CategoryId,
                    Path = root.Path,
                    Name = root.Name,
                    Label = root.Label,
                    Component = root.Component,
                    Meta = new NodeMeta { Icon = root.Icon, Title = root.Name },
                    AlwaysShow = root.AlwaysShow,
                    Hidden = root.Hidden,
                    Sort = root.Sort,
                    Icon = root.Icon,
                    Permission = root.Permission,
                    ClientType = root.ClientType,
                    Business = root.Business,
                    OtherPlatformCode = root.OtherPlatformCode

                };
                if (menus.Where(_ => _.Pid == root.Id).Any())
                {
                    menu.Children = LoadMenusTree(menus.Where(_ => _.Pid == root.Id).OrderBy(_ => _.Sort).ToList(), menus);
                }
                else
                {
                    menu.Children = new List<MenuNodesDto>();
                }
                result.Add(menu);
            }
            return result;
        }

    }
}
