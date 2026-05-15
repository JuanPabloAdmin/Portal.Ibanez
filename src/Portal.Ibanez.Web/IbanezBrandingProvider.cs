using Microsoft.Extensions.Localization;
using Portal.Ibanez.Localization;
using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;

namespace Portal.Ibanez.Web;

[Dependency(ReplaceServices = true)]
public class IbanezBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<IbanezResource> _localizer;

    public IbanezBrandingProvider(IStringLocalizer<IbanezResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
