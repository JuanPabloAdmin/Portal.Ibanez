using Xunit;

namespace Portal.Ibanez.EntityFrameworkCore;

[CollectionDefinition(IbanezTestConsts.CollectionDefinitionName)]
public class IbanezEntityFrameworkCoreCollection : ICollectionFixture<IbanezEntityFrameworkCoreFixture>
{

}
