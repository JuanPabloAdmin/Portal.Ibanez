using System;
using System.Collections.Generic;
using System.Text;
using Portal.Ibanez.Localization;
using Volo.Abp.Application.Services;

namespace Portal.Ibanez;

/* Inherit your application services from this class.
 */
public abstract class IbanezAppService : ApplicationService
{
    protected IbanezAppService()
    {
        LocalizationResource = typeof(IbanezResource);
    }
}
