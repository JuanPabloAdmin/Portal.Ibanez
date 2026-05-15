using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Portal.Ibanez.Data;
using Volo.Abp.DependencyInjection;

namespace Portal.Ibanez.EntityFrameworkCore;

public class EntityFrameworkCoreIbanezDbSchemaMigrator
    : IIbanezDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreIbanezDbSchemaMigrator(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolve the IbanezDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<IbanezDbContext>()
            .Database
            .MigrateAsync();
    }
}
