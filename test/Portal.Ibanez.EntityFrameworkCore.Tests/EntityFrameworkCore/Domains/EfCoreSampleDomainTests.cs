using Portal.Ibanez.Samples;
using Xunit;

namespace Portal.Ibanez.EntityFrameworkCore.Domains;

[Collection(IbanezTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<IbanezEntityFrameworkCoreTestModule>
{

}
