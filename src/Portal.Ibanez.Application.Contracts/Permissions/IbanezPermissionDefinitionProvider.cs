using Portal.Ibanez.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Portal.Ibanez.Permissions;

public class IbanezPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(IbanezPermissions.GroupName);
        //Define your own permissions here. Example:
        //myGroup.AddPermission(IbanezPermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<IbanezResource>(name);
    }
}
