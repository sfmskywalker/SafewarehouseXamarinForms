using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SafeWarehouseApp.Persistence
{
    public class SafeWarehouseContextFactory : IDesignTimeDbContextFactory<SafeWarehouseContext>
    {
        public SafeWarehouseContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<SafeWarehouseContext>();
            var connectionString = args.Any() ? args[0] : "Data Source=veiligmagazijn.db;Cache=Shared";
            builder.UseSqlite(connectionString, db => db.MigrationsAssembly(typeof(SafeWarehouseContextFactory).Assembly.GetName().Name));
            return new SafeWarehouseContext(builder.Options);
        }
    }
}