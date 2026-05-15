using Portal.Ibanez.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Portal.Ibanez.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class IbanezController : AbpControllerBase
{
    protected IbanezController()
    {
        LocalizationResource = typeof(IbanezResource);
    }
}
