using Microsoft.EntityFrameworkCore;
using Portal.Ibanez.Customers;
using Portal.Ibanez.Documents;
using Portal.Ibanez.Machines;
using Portal.Ibanez.QrCodes;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;


namespace Portal.Ibanez.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ReplaceDbContext(typeof(ITenantManagementDbContext))]
[ConnectionStringName("Default")]
public class IbanezDbContext :
    AbpDbContext<IbanezDbContext>,
    IIdentityDbContext,
    ITenantManagementDbContext
{
    /* Add DbSet properties for your Aggregate Roots / Entities here. */

    #region Entities from the modules

    /* Notice: We only implemented IIdentityDbContext and ITenantManagementDbContext
     * and replaced them for this DbContext. This allows you to perform JOIN
     * queries for the entities of these modules over the repositories easily. You
     * typically don't need that for other modules. But, if you need, you can
     * implement the DbContext interface of the needed module and use ReplaceDbContext
     * attribute just like IIdentityDbContext and ITenantManagementDbContext.
     *
     * More info: Replacing a DbContext of a module ensures that the related module
     * uses this DbContext on runtime. Otherwise, it will use its own DbContext class.
     */

    //Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }
    public DbSet<IdentitySession> Sessions { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<MachineType> MachineTypes { get; set; }
    public DbSet<Machine> Machines { get; set; }
    public DbSet<MachineDocument> MachineDocuments { get; set; }
    public DbSet<QrCode> QrCodes { get; set; }
    public DbSet<QrCodeDocument> QrCodeDocuments { get; set; }
    // Tenant Management
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }

    #endregion

    public IbanezDbContext(DbContextOptions<IbanezDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureFeatureManagement();
        builder.ConfigureTenantManagement();

        /* Configure your own tables/entities inside here */

        //builder.Entity<YourEntity>(b =>
        //{
        //    b.ToTable(IbanezConsts.DbTablePrefix + "YourEntities", IbanezConsts.DbSchema);
        //    b.ConfigureByConvention(); //auto configure for the base class props
        //    //...
        //});
        builder.Entity<Customer>(b =>
        {
            b.ToTable(IbanezConsts.DbTablePrefix + "Customers", IbanezConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.CommercialName).IsRequired().HasMaxLength(200);
            b.Property(x => x.FiscalName).HasMaxLength(200);
            b.Property(x => x.TaxId).HasMaxLength(50);
            b.Property(x => x.Address).HasMaxLength(500);
            b.Property(x => x.Phone).HasMaxLength(50);
            b.Property(x => x.Email).HasMaxLength(200);
            b.Property(x => x.ContactPerson).HasMaxLength(200);
        });

        builder.Entity<MachineType>(b =>
        {
            b.ToTable(IbanezConsts.DbTablePrefix + "MachineTypes", IbanezConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Name).IsRequired().HasMaxLength(200);
        });

        builder.Entity<Machine>(b =>
        {
            b.ToTable(IbanezConsts.DbTablePrefix + "Machines", IbanezConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.OrderNumber).HasMaxLength(100);
            b.Property(x => x.CabinetNumber).HasMaxLength(100);
            b.Property(x => x.Observations).HasMaxLength(1000);
        });

        builder.Entity<MachineDocument>(b =>
        {
            b.ToTable(IbanezConsts.DbTablePrefix + "MachineDocuments", IbanezConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Title).IsRequired().HasMaxLength(200);
            b.Property(x => x.FileName).IsRequired().HasMaxLength(255);
            b.Property(x => x.StoredFileName).IsRequired().HasMaxLength(255);
            b.Property(x => x.ContentType).IsRequired().HasMaxLength(100);
        });

        builder.Entity<QrCode>(b =>
        {
            b.ToTable(IbanezConsts.DbTablePrefix + "QrCodes", IbanezConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Code).IsRequired().HasMaxLength(100);
            b.Property(x => x.Name).IsRequired().HasMaxLength(200);
            b.Property(x => x.Description).HasMaxLength(1000);

            b.HasIndex(x => x.Code).IsUnique();
        });

        builder.Entity<QrCodeDocument>(b =>
        {
            b.ToTable(IbanezConsts.DbTablePrefix + "QrCodeDocuments", IbanezConsts.DbSchema);
            b.ConfigureByConvention();

            b.HasIndex(x => new { x.QrCodeId, x.MachineDocumentId }).IsUnique();
        });
    }
}
