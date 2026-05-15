using Portal.Ibanez.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Portal.Ibanez.Web.Pages;

/* Inherit your PageModel classes from this class.
 */
public abstract class IbanezPageModel : AbpPageModel
{
    protected IbanezPageModel()
    {
        LocalizationResourceType = typeof(IbanezResource);
    }
}
