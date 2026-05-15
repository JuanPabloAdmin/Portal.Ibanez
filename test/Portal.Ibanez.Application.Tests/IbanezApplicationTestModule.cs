using Volo.Abp.Modularity;

namespace Portal.Ibanez;

[DependsOn(
    typeof(IbanezApplicationModule),
    typeof(IbanezDomainTestModule)
)]
public class IbanezApplicationTestModule : AbpModule
{

}
