using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Portal.Ibanez.Data;

/* This is used if database provider does't define
 * IIbanezDbSchemaMigrator implementation.
 */
public class NullIbanezDbSchemaMigrator : IIbanezDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
