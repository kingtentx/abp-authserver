using BaseService.BaseData;
using BaseService.ExtModels;
using BaseService.Systems;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.Identity;

namespace BaseService.EntityFrameworkCore
{
    [ConnectionStringName("Default")]
    public class BaseServiceDbContext : AbpDbContext<BaseServiceDbContext>
    {
        public DbSet<DataDictionary> DataDictionaries { get; set; }
        public DbSet<DataDictionaryDetail> DataDictionaryDetails { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<UserPosition> UserPosition { get; set; }
        public DbSet<UserOrganization> UserOrganization { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<RoleMenu> RoleMenu { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRole { get; set; }
        public DbSet<Authority> Authoritys { get; set; }
        public DbSet<AuthorityDetail> AuthorityDetails { get; set; }
        public DbSet<RoleAuthority> RoleAuthority { get; set; }
        public DbSet<GatewayConfig> EdgeConfig { get; set; }      
        public DbSet<AuthorityGroup> AuthorityGroup { get; set; }
        public DbSet<UserFeature> UserFeature { get; set; }

        public BaseServiceDbContext(DbContextOptions<BaseServiceDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>(b =>
            {
                b.ToTable(AbpIdentityDbProperties.DbTablePrefix + "Users");
                b.ConfigureByConvention();//扩展字段ExtraProperties             
            });

            builder.Entity<Role>(b =>
            {
                b.ToTable(AbpIdentityDbProperties.DbTablePrefix + "Roles");
                b.ConfigureByConvention();//扩展字段ExtraProperties               
            });

            builder.Entity<UserRole>(b =>
            {
                b.ToTable(AbpIdentityDbProperties.DbTablePrefix + "UserRoles");             
                b.HasKey(k => new { k.UserId, k.RoleId });
            });

            builder.ConfigureBaseService();
        }
    }
}
