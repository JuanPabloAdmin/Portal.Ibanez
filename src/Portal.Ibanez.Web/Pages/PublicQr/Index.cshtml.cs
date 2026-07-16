using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portal.Ibanez.QrCodes;

namespace Portal.Ibanez.Web.Pages.PublicQr;

[AllowAnonymous]
public class IndexModel : IbanezPageModel
{
    [BindProperty(SupportsGet = true)]
    public string Code { get; set; } = string.Empty;

    public void OnGet(string code)
    {
        Code = code;
    }
}