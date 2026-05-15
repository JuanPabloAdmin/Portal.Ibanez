using Volo.Abp.Modularity;

namespace Portal.Ibanez;

/* Inherit from this class for your domain layer tests. */
public abstract class IbanezDomainTestBase<TStartupModule> : IbanezTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
