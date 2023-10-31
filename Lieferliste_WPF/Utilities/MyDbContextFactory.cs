using El2Core.Models;
using Lieferliste_WPF.Properties;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Lieferliste_WPF.Utilities
{
    public class MyDbContextFactory : IDesignTimeDbContextFactory<DB_COS_LIEFERLISTE_SQLContext>
    {
        public DB_COS_LIEFERLISTE_SQLContext CreateDbContext(string[]? args = null)
        {
            string defaultConnection = Settings.Default.ConnectionBosch;

            var optionsBuilder = new DbContextOptionsBuilder<DB_COS_LIEFERLISTE_SQLContext>();
            optionsBuilder.UseSqlServer(defaultConnection);

            return new DB_COS_LIEFERLISTE_SQLContext(optionsBuilder.Options);
        }
    }
}
