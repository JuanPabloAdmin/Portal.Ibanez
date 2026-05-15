using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Portal.Ibanez.Web.Pages;

[Authorize]
public class IndexModel : IbanezPageModel
{
    public IActionResult OnGet()
    {
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            return Redirect("/Account/Login");
        }

        return Redirect("/Customers");
    }

}
