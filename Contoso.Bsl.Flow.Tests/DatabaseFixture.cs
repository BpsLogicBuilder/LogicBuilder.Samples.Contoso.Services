using Microsoft.Data.SqlClient;
using System;
using System.Threading.Tasks;
using Testcontainers.MsSql;

namespace Contoso.Bsl.Flow.Tests
{
    public class DatabaseFixture : IAsyncLifetime
    {
        private readonly MsSqlContainer _msSqlContainer = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2025-latest")//2025-latest
                                .Build();

        public string GetConnectionString(string initialCatalog)
        {
            return new SqlConnectionStringBuilder(_msSqlContainer.GetConnectionString())
            {
                InitialCatalog = initialCatalog
            }.ToString();
        }

        async ValueTask IAsyncLifetime.InitializeAsync()
        {
            await _msSqlContainer.StartAsync();
        }

        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            if (_msSqlContainer != null)
                await _msSqlContainer.DisposeAsync();

            GC.SuppressFinalize(this);
        }
    }
}
