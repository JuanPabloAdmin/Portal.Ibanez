using Portal.Ibanez.Localization;
using Portal.Ibanez.Web.Models;
using System.Collections.Generic;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Portal.Ibanez.Web.Pages;

public abstract class IbanezPageModel : AbpPageModel
{
    public List<BreadcrumbItem> Breadcrumbs { get; set; } = new();

    protected IbanezPageModel()
    {
        LocalizationResourceType = typeof(IbanezResource);
    }
}