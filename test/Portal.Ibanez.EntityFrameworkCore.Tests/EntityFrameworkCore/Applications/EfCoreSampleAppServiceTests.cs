using Portal.Ibanez.Samples;
using Xunit;

namespace Portal.Ibanez.EntityFrameworkCore.Applications;

[Collection(IbanezTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<IbanezEntityFrameworkCoreTestModule>
{

}
