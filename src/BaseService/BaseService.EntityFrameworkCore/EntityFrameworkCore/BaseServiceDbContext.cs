using BaseService.BaseData;
using BaseService.ExtModels;
using BaseService.Systems;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace BaseService.EntityFrameworkCore
{
    [ConnectionStringName("Default")]
    public class BaseServiceDbContext : AbpDbContext<BaseServiceDbContext>
    {
        public DbSet<DataDictionary> DataDictionaries { get; set; }
        public DbSet<DataDictionaryDetail> DataDictionaryDetails { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Position> Jobs { get; set; }
        public DbSet<UserPosition> UserJobs { get; set; }
        public DbSet<UserOrganization> UserOrganizations { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<RoleMenu> RoleMenus { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> AppUserRoles { get; set; }
        public DbSet<Authority> Authority { get; set; }
        public DbSet<AuthorityDetail> AuthorityDetail { get; set; }
        public DbSet<RoleAuthority> RoleAuthority { get; set; }
        public DbSet<EdgeConfig> EdgeConfig { get; set; }
        public DbSet<AuthorityEdge> AuthorityEdge { get; set; }
        public DbSet<AuthorityGroup> AuthorityGroup { get; set; }



        public BaseServiceDbContext(DbContextOptions<BaseServiceDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ConfigureBaseService();

            builder.ConfigurePermissionManagement();
            builder.ConfigureSettingManagement();
            //builder.ConfigureAuditLogging();
            builder.ConfigureIdentity();
            builder.ConfigureTenantManagement();
         

            //builder.Entity<User>(b =>
            //{
            //    b.ToTable(AbpIdentityDbProperties.DbTablePrefix + "Users");
            //    b.ConfigureByConvention();//扩展字段ExtraProperties             
            //});

            //builder.Entity<Role>(b =>
            //{
            //    b.ToTable(AbpIdentityDbProperties.DbTablePrefix + "Roles");
            //    b.ConfigureByConvention();//扩展字段ExtraProperties               
            //});

            //builder.Entity<UserRole>(b =>
            //{
            //    b.ToTable(AbpIdentityDbProperties.DbTablePrefix + "UserRoles");             
            //    b.HasKey(k => new { k.UserId, k.RoleId });
            //});

            //builder.ConfigureBaseService();

            
        }
    }
}
