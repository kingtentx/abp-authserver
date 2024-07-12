using BaseService.BaseData;
using BaseService.ExtModels;
using BaseService.Systems;
using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace BaseService.EntityFrameworkCore
{
    public static class BaseDbContextModelCreatingExtensions
    {
        public static void ConfigureBaseService(this ModelBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            builder.Entity<DataDictionary>(b =>
            {
                b.ToTable("base_dict");
                b.ConfigureByConvention();
                b.HasIndex(q => q.Name);
            });

            builder.Entity<DataDictionaryDetail>(b =>
            {
                b.ToTable("base_dict_details");
                b.ConfigureByConvention();
                b.HasIndex(q => q.DictionaryId);
            });

            builder.Entity<Organization>(b =>
            {
                b.ToTable("base_orgs");
                b.ConfigureByConvention();
                b.HasIndex(q => q.Pid);
            });

            builder.Entity<Position>(b =>
            {
                b.ToTable("base_positions");
                b.ConfigureByConvention();
                b.HasIndex(q => q.Name);
            });

            builder.Entity<UserPosition>(b =>
            {
                b.ToTable("base_user_positions");
                b.HasKey(k => new { k.UserId, k.PositionId });
            });

            builder.Entity<UserOrganization>(b =>
            {
                b.ToTable("base_user_orgs");
                b.HasKey(k => new { k.UserId, k.OrganizationId });
            });

            builder.Entity<Menu>(b =>
            {
                b.ToTable("sys_menu");
                b.ConfigureByConvention();
            });

            builder.Entity<RoleMenu>(b =>
            {
                b.ToTable("sys_role_menu");
                b.HasKey(k => new { k.RoleId, k.MenuId });
            });

            #region  Authority

            builder.Entity<Authority>(b =>
            {
                b.ToTable("sys_authority");
                b.ConfigureByConvention();
            });

            builder.Entity<AuthorityDetail>(b =>
            {
                b.ToTable("sys_authority_detail");
                b.ConfigureByConvention();

            });

            builder.Entity<RoleAuthority>(b =>
            {
                b.ToTable("sys_role_authority");
                b.HasKey(k => new { k.RoleId, k.AuthorityId });
            });

            builder.Entity<GatewayConfig>(b =>
            {
                b.ToTable("sys_gateway_config");
                b.ConfigureByConvention();
            });

          
            builder.Entity<AuthorityGroup>(b =>
            {
                b.ToTable("sys_authority_group");
                b.ConfigureByConvention();

            });

            #endregion

            builder.Entity<UserFeature>(b =>
            {
                b.ToTable("base_user_feature");
                b.ConfigureByConvention();
            });
        }
    }
}
