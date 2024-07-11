using BaseService.ExtModels;
using BaseService.Systems;
using Microsoft.EntityFrameworkCore;
using System;
using Volo.Abp.Identity;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Threading;

namespace BaseService.EntityFrameworkCore
{
    public class BaseEfCoreEntityExtensionMappings
    {
        private static readonly OneTimeRunner OneTimeRunner = new OneTimeRunner();

        public static void Configure()
        {
            BaseServiceModuleExtensionConfigurator.Configure();

            OneTimeRunner.Run(() =>
            {
                ObjectExtensionManager.Instance
                  //User
                  .MapEfCoreProperty<IdentityUser, string>(nameof(User.Sex),
                    (entityBuilder, propertyBuilder) =>
                    {
                        propertyBuilder.HasMaxLength(ModelUnits.Len_10);
                        propertyBuilder.HasComment("性别");
                    })
                   .MapEfCoreProperty<IdentityUser, string>(nameof(User.JobNo),
                   (entityBuilder, propertyBuilder) =>
                   {
                       propertyBuilder.HasMaxLength(ModelUnits.Len_20);
                       propertyBuilder.HasComment("工号");
                   })
                  .MapEfCoreProperty<IdentityUser, Guid?>(nameof(User.AuthorityId),
                   (entityBuilder, propertyBuilder) =>
                   {
                       propertyBuilder.HasDefaultValue(null);
                       propertyBuilder.HasComment("权限ID");
                   })

                  //Role                
                  .MapEfCoreProperty<IdentityRole, int>(nameof(Role.Sort),
                  (entityBuilder, propertyBuilder) =>
                  {                     
                      propertyBuilder.IsRequired(true).HasDefaultValue(0);
                      propertyBuilder.HasComment("排序");
                  })
                  .MapEfCoreProperty<IdentityRole, string>(nameof(Role.Remark),
                  (entityBuilder, propertyBuilder) =>
                  {
                      propertyBuilder.HasMaxLength(ModelUnits.Len_500);
                      propertyBuilder.HasComment("备注");
                  })
                  .MapEfCoreProperty<IdentityRole, bool>(nameof(Role.IsActive),
                  (entityBuilder, propertyBuilder) =>
                  {
                      propertyBuilder.IsRequired(true).HasDefaultValue(false);
                      propertyBuilder.HasComment("是否激活");
                  })
                  .MapEfCoreProperty<IdentityRole, Guid?>(nameof(Role.AuthorityId),
                  (entityBuilder, propertyBuilder) =>
                  {
                      propertyBuilder.HasDefaultValue(null);
                      propertyBuilder.HasComment("权限ID");
                  })
                 ;


            });
        }
    }
}
