using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Internal;

namespace EShop.Tests;

#pragma warning disable EF1001 // MigrationsAssembly 是 EF Core 的內部 API
public class NoMigrationsAssembly(
    ICurrentDbContext currentContext,
    IDbContextOptions options,
    IMigrationsIdGenerator idGenerator,
    IDiagnosticsLogger<DbLoggerCategory.Migrations> logger)
    : MigrationsAssembly(currentContext, options, idGenerator, logger)
{
    public override IReadOnlyDictionary<string, TypeInfo> Migrations { get; } =
        new Dictionary<string, TypeInfo>();

    public override ModelSnapshot? ModelSnapshot => null;
}
#pragma warning restore EF1001
