using BaseService.Enums;
using BaseService.Permissions;
using BaseService.Systems.MenuManagement.Dto;
using Cimc.Model.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Serilog;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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

            //var query = await _repository.FindAsync(p => p.Title.Equals(input.Label.Trim()));
            //if (query != null)
            //{
            //    result.Message = $"[{input.Label}] 菜单代码已存在。";
            //    return result;
            //}

            var menu = await _repository.InsertAsync(new Menu(GuidGenerator.Create())
            {
                MenuType = input.MenuType,
                ParentId = input.ParentId,
                HigherMenuOptions = input.HigherMenuOptions,
                Title = input.Title?.Trim(),
                Route = input.Route?.Trim(),
                Component = input.Component,
                Sort = input.Sort,
                Path = input.Path?.Trim(),
                Redirect = input.Redirect?.Trim(),
                Icon = input.Icon,
                ExtraIcon = input.ExtraIcon,
                EnterTransition = input.EnterTransition,
                LeaveTransition = input.LeaveTransition,
                ActivePath = input.ActivePath,
                Auths = input.Auths?.Trim(),
                FrameSrc = input.FrameSrc?.Trim(),
                FrameLoading = input.FrameLoading?.Trim(),
                KeepAlive = input.KeepAlive,
                HiddenTag = input.HiddenTag,
                FixedTag = input.FixedTag,
                ShowLink = input.ShowLink,
                ShowParent = input.ShowParent,
                ClientType = input.ClientType
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

            //var query = await _repository.FindAsync(p => p.Label.Equals(input.Label.Trim()) && p.Id != input.Id);
            //if (query != null)
            //{
            //    result.Message = $"[{input.Label}] 菜单代码已存在。";
            //    return result;
            //}

            var menu = await _repository.GetAsync(input.Id.Value);
            menu.MenuType = input.MenuType;
            menu.ParentId = input.ParentId;
            menu.HigherMenuOptions = input.HigherMenuOptions;
            menu.Title = input.Title?.Trim();
            menu.Route = input.Route?.Trim();
            menu.Component = input.Component;
            menu.Sort = input.Sort;
            menu.Path = input.Path?.Trim();
            menu.Redirect = input.Redirect?.Trim();
            menu.Icon = input.Icon;
            menu.ExtraIcon = input.ExtraIcon;
            menu.EnterTransition = input.EnterTransition;
            menu.LeaveTransition = input.LeaveTransition;
            menu.ActivePath = input.ActivePath;
            menu.Auths = input.Auths?.Trim();
            menu.FrameSrc = input.FrameSrc?.Trim();
            menu.FrameLoading = input.FrameLoading?.Trim();
            menu.KeepAlive = input.KeepAlive;
            menu.HiddenTag = input.HiddenTag;
            menu.FixedTag = input.FixedTag;
            menu.ShowLink = input.ShowLink;
            menu.ShowParent = input.ShowParent;
            menu.ClientType = input.ClientType;

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

            var query = (await _repository.GetQueryableAsync()).WhereIf(!string.IsNullOrWhiteSpace(input.Filter), _ => _.Title.Contains(input.Filter));
            var items = await query.OrderBy(input.Sorting ?? nameof(Menu.Sort))
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

            //if (dto.ParentId.HasValue)
            //    dto.ParentLabel = (await _repository.FirstOrDefaultAsync(_ => _.Id == query.ParentId))?.Label;

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
                var root = menus.Where(_ => _.ParentId == null).OrderBy(_ => _.Sort).ToList();
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
                    MenuType = root.MenuType,
                    ParentId = root.ParentId,
                    HigherMenuOptions = root.HigherMenuOptions,
                    Title = root.Title?.Trim(),
                    Route = root.Route?.Trim(),
                    Component = root.Component,
                    Sort = root.Sort,
                    Path = root.Path?.Trim(),
                    Redirect = root.Redirect?.Trim(),
                    Icon = root.Icon,
                    ExtraIcon = root.ExtraIcon,
                    EnterTransition = root.EnterTransition,
                    LeaveTransition = root.LeaveTransition,
                    ActivePath = root.ActivePath,
                    Auths = root.Auths?.Trim(),
                    FrameSrc = root.FrameSrc?.Trim(),
                    FrameLoading = root.FrameLoading?.Trim(),
                    KeepAlive = root.KeepAlive,
                    HiddenTag = root.HiddenTag,
                    FixedTag = root.FixedTag,
                    ShowLink = root.ShowLink,
                    ShowParent = root.ShowParent,

                };
                if (menus.Where(_ => _.ParentId == root.Id).Any())
                {
                    menu.Children = LoadMenusTree(menus.Where(_ => _.ParentId == root.Id).OrderBy(_ => _.Sort).ToList(), menus);
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
