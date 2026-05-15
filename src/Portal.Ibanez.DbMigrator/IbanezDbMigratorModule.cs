using Portal.Ibanez.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace Portal.Ibanez.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(IbanezEntityFrameworkCoreModule),
    typeof(IbanezApplicationContractsModule)
    )]
public class IbanezDbMigratorModule : AbpModule
{
}
