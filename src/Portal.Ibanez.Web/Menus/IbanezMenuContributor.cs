using Microsoft.Extensions.DependencyInjection;
using Portal.Ibanez.Localization;
using Portal.Ibanez.MultiTenancy;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Identity.Web.Navigation;
using Volo.Abp.SettingManagement.Web.Navigation;
using Volo.Abp.TenantManagement.Web.Navigation;
using Volo.Abp.UI.Navigation;
using Volo.Abp.Users;

namespace Portal.Ibanez.Web.Menus;

public class IbanezMenuContributor : IMenuContributor
{
    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        var currentUser = context.ServiceProvider.GetRequiredService<ICurrentUser>();

        if (!currentUser.IsAuthenticated)
        {
            return;
        }

        if (!currentUser.Roles.Contains("admin"))
        {
            return;
        }

        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
    }

    private Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        var administration = context.Menu.GetAdministration();
        var l = context.GetLocalizer<IbanezResource>();

        context.Menu.Items.Insert(
            0,
            new ApplicationMenuItem(
                IbanezMenus.Home,
                l["Menu:Home"],
                "~/",
                icon: "fas fa-home",
                order: 0
            )
        );
        context.Menu.AddItem(
    new ApplicationMenuItem(
        "Ibanez.Customers",
        "Clientes",
        url: "/Customers",
        icon: "fa fa-users",
        order: 1
    )
);

        if (MultiTenancyConsts.IsEnabled)
        {
            administration.SetSubItemOrder(TenantManagementMenuNames.GroupName, 1);
        }
        else
        {
            administration.TryRemoveMenuItem(TenantManagementMenuNames.GroupName);
        }

        administration.SetSubItemOrder(IdentityMenuNames.GroupName, 2);
        administration.SetSubItemOrder(SettingManagementMenuNames.GroupName, 3);

        return Task.CompletedTask;
    }
}
