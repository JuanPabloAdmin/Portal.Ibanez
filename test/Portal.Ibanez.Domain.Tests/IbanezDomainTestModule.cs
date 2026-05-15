using Volo.Abp.Modularity;

namespace Portal.Ibanez;

[DependsOn(
    typeof(IbanezDomainModule),
    typeof(IbanezTestBaseModule)
)]
public class IbanezDomainTestModule : AbpModule
{

}
