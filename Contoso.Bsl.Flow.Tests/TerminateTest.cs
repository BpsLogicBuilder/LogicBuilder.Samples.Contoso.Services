using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using Contoso.Bsl.Flow.Interfaces;
using Contoso.BSL.AutoMapperProfiles;
using Contoso.Contexts;
using Contoso.Repositories;
using LogicBuilder.EntityFrameworkCore.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading.Tasks;

namespace Contoso.Bsl.Flow.Tests
{
    public class TerminateTest : IClassFixture<DatabaseFixture>
    {
        static TerminateTest()
        {
            InitializeMapperConfiguration();
        }

        public TerminateTest(DatabaseFixture databaseFixture, ITestOutputHelper output)
        {
            this.databaseFixture = databaseFixture;
            this.output = output;
            Initialize();
        }

        #region Fields
        private readonly DatabaseFixture databaseFixture;
        private readonly ITestOutputHelper output;
        private static IServiceProvider? serviceProvider;
        private static MapperConfiguration MapperConfiguration;
        #endregion Fields

        [Fact]
        public async Task CallTerminateFlow()
        {
            //arrange
            IFlowManager flowManager = serviceProvider!.GetRequiredService<IFlowManager>();

            //act
            System.Diagnostics.Stopwatch stopWatch = System.Diagnostics.Stopwatch.StartNew();
            flowManager.Start("callterminate");
            stopWatch.Stop();
            this.output.WriteLine("Terminate flow processed in time = {0}", stopWatch.Elapsed.TotalMilliseconds);

            //assert
            Assert.False(flowManager.FlowDataCache.Response!.Success);
        }

        #region Helpers
        [MemberNotNull(nameof(MapperConfiguration))]
        private static void InitializeMapperConfiguration()
        {
            MapperConfiguration ??= ConfigurationHelper.GetMapperConfiguration(cfg =>
            {
                cfg.AddExpressionMapping();

                cfg.AddProfile<ExpressionOperatorsMappingProfile>();
                cfg.AddProfile<ExpressionParameterToDescriptorMappingProfile>();
                cfg.AddProfile<ExpansionParameterToDescriptorMappingProfile>();
                cfg.AddProfile<ExpansionDescriptorToOperatorMappingProfile>();
                cfg.AddProfile<SchoolProfile>();
            });
            MapperConfiguration.AssertConfigurationIsValid();
        }

        [MemberNotNull(nameof(serviceProvider))]
        private void Initialize()
        {
            serviceProvider ??= new ServiceCollection()
                .AddDbContext<SchoolContext>
                (
                    options => options.UseSqlServer
                    (
                        databaseFixture.GetConnectionString(GetType().Name),
                        options => options.EnableRetryOnFailure()
                    ),
                    ServiceLifetime.Transient
                )
                .AddLogging()
                .AddContosoBslFlowServices()
                .AddSingleton<AutoMapper.IConfigurationProvider>
                (
                    MapperConfiguration
                )
                .AddTransient<IMapper>(sp => new Mapper(sp.GetRequiredService<AutoMapper.IConfigurationProvider>(), sp.GetService))
                .BuildServiceProvider();

            ReCreateDataBase(serviceProvider.GetRequiredService<SchoolContext>()).GetAwaiter().GetResult();
            DatabaseSeeder.Seed_Database(serviceProvider.GetRequiredService<ISchoolRepository>()).GetAwaiter().GetResult();
        }

        private static async Task ReCreateDataBase(SchoolContext context)
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }
        #endregion Helpers
    }
}
