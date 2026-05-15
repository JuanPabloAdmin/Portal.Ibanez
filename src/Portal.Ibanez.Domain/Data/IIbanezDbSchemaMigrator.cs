using System.Threading.Tasks;

namespace Portal.Ibanez.Data;

public interface IIbanezDbSchemaMigrator
{
    Task MigrateAsync();
}
