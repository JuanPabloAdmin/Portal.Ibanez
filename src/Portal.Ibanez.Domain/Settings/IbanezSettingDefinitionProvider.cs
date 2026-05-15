using Volo.Abp.Settings;

namespace Portal.Ibanez.Settings;

public class IbanezSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(IbanezSettings.MySetting1));
    }
}
