using Volo.Abp.Modularity;

namespace Portal.Ibanez;

public abstract class IbanezApplicationTestBase<TStartupModule> : IbanezTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
