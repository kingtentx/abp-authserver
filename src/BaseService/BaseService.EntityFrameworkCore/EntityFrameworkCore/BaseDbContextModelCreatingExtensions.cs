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
                b.ToTable("base_menu");
                b.ConfigureByConvention();
            });

            builder.Entity<RoleMenu>(b =>
            {
                b.ToTable("base_role_menu");
                b.HasKey(k => new { k.RoleId, k.MenuId });
            });

            #region  Authority

            builder.Entity<Authority>(b =>
            {
                b.ToTable("base_authority");
                b.ConfigureByConvention();
            });

            builder.Entity<AuthorityDetail>(b =>
            {
                b.ToTable("base_authority_detail");
                b.ConfigureByConvention();

            });

            builder.Entity<RoleAuthority>(b =>
            {
                b.ToTable("base_role_authority");
                b.HasKey(k => new { k.RoleId, k.AuthorityId });
            });

            builder.Entity<EdgeConfig>(b =>
            {
                b.ToTable("base_edge_config");
                b.ConfigureByConvention();
            });

            builder.Entity<AuthorityEdge>(b =>
            {
                b.ToTable("base_authority_edge");
                b.HasKey(k => new { k.EdgeId, k.AuthorityId });
            });
            builder.Entity<AuthorityGroup>(b =>
            {
                b.ToTable("base_authority_group");
                b.ConfigureByConvention();

            });

            #endregion


        }
    }
}
