using BaseService.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace BaseService.Permissions
{
    public class BaseServicePermissionDefinitionProvider : PermissionDefinitionProvider
    {

        public override void Define(IPermissionDefinitionContext context)
        {
            var permission = context.AddGroup(BaseServicePermissions.BaseService, L("BaseService"), MultiTenancySides.Tenant);

            var auditLogging = permission.AddPermission(BaseServicePermissions.AuditLogging.Default, L("AuditLogging"));
            //字典
            var dictionary = permission.AddPermission(BaseServicePermissions.DataDictionary.Default, L("DataDictionary"));
            dictionary.AddChild(BaseServicePermissions.DataDictionary.Update, L("Edit"));
            dictionary.AddChild(BaseServicePermissions.DataDictionary.Delete, L("Delete"));
            dictionary.AddChild(BaseServicePermissions.DataDictionary.Create, L("Create"));
            //菜单
            var menu = permission.AddPermission(BaseServicePermissions.Menu.Default, L("Menu"));
            menu.AddChild(BaseServicePermissions.Menu.Update, L("Edit"));
            menu.AddChild(BaseServicePermissions.Menu.Delete, L("Delete"));
            menu.AddChild(BaseServicePermissions.Menu.Create, L("Create"));

            //角色菜单
            var roleMenu = permission.AddPermission(BaseServicePermissions.RoleMenu.Default, L("Menu"));
            roleMenu.AddChild(BaseServicePermissions.RoleMenu.Update, L("Edit"));

            //机构
            var organization = permission.AddPermission(BaseServicePermissions.Organization.Default, L("Organization"));
            organization.AddChild(BaseServicePermissions.Organization.Update, L("Edit"));
            organization.AddChild(BaseServicePermissions.Organization.Delete, L("Delete"));
            organization.AddChild(BaseServicePermissions.Organization.Create, L("Create"));
            //岗位
            var position = permission.AddPermission(BaseServicePermissions.Position.Default, L("Position"));
            position.AddChild(BaseServicePermissions.Position.Update, L("Edit"));
            position.AddChild(BaseServicePermissions.Position.Delete, L("Delete"));
            position.AddChild(BaseServicePermissions.Position.Create, L("Create"));

            //权限对象
            var authority = permission.AddPermission(BaseServicePermissions.Authority.Default, L("Position"));
            authority.AddChild(BaseServicePermissions.Authority.Update, L("Edit"));
            authority.AddChild(BaseServicePermissions.Authority.Delete, L("Delete"));
            authority.AddChild(BaseServicePermissions.Authority.Create, L("Create"));

            //网关
            var edge = permission.AddPermission(BaseServicePermissions.Edge.Default, L("Position"));
            edge.AddChild(BaseServicePermissions.Edge.Update, L("Edit"));
            edge.AddChild(BaseServicePermissions.Edge.Delete, L("Delete"));
            edge.AddChild(BaseServicePermissions.Edge.Create, L("Create"));

            ////设备模型
            //var deviceModel = permission.AddPermission(BaseServicePermissions.DeviceModel.Default, L("Position"));
            //deviceModel.AddChild(BaseServicePermissions.DeviceModel.Update, L("Edit"));
            //deviceModel.AddChild(BaseServicePermissions.DeviceModel.Delete, L("Delete"));
            //deviceModel.AddChild(BaseServicePermissions.DeviceModel.Create, L("Create"));

            ////设备
            //var device = permission.AddPermission(BaseServicePermissions.Device.Default, L("Position"));
            //device.AddChild(BaseServicePermissions.Device.Update, L("Edit"));
            //device.AddChild(BaseServicePermissions.Device.Delete, L("Delete"));
            //device.AddChild(BaseServicePermissions.Device.Create, L("Create"));
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<BaseServiceResource>(name);
        }
    }
}
