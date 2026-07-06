using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using Contoso.Bsl.Flow.Interfaces;
using Contoso.BSL.AutoMapperProfiles;
using Contoso.Contexts;
using Contoso.Data.Entities;
using Contoso.Domain.Entities;
using Contoso.Repositories;
using LogicBuilder.App.Bsl.Business.Requests;
using LogicBuilder.App.Bsl.Business.Responses;
using LogicBuilder.EntityFrameworkCore.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Contoso.Bsl.Flow.Tests
{
    public class SaveDepartmentTest : IClassFixture<DatabaseFixture>
    {
        static SaveDepartmentTest()
        {
            InitializeMapperConfiguration();
        }

        public SaveDepartmentTest(DatabaseFixture databaseFixture, ITestOutputHelper output)
        {
            this.databaseFixture = databaseFixture;
            this.output = output;
            Initialize();
        }

        #region Fields
        private readonly DatabaseFixture databaseFixture;
        private readonly ITestOutputHelper output;
        private static MapperConfiguration MapperConfiguration;
        private static IServiceProvider? serviceProvider;
        #endregion Fields

        [Fact]
        public async Task SaveDepartment()
        {
            //arrange
            IFlowManager flowManager = serviceProvider!.GetRequiredService<IFlowManager>();
            ISchoolRepository schoolRepository = serviceProvider!.GetRequiredService<ISchoolRepository>();
            var department = (await schoolRepository.GetAsync<DepartmentModel, Department>
            (
                s => s.Name == "Mathematics"
            )).Single();
            department.Budget = 1000.1m;
            department.EntityState = LogicBuilder.Domain.EntityStateType.Modified;
            flowManager.FlowDataCache.Request = new SaveEntityRequest { Entity = department };

            //act
            System.Diagnostics.Stopwatch stopWatch = System.Diagnostics.Stopwatch.StartNew();
            flowManager.Start("savedepartment");
            stopWatch.Stop();
            this.output.WriteLine("Saving valid department  = {0}", stopWatch.Elapsed.TotalMilliseconds);

            //assert
            Assert.True(flowManager.FlowDataCache.Response!.Success);
            Assert.Equal(1000.1m, ((DepartmentModel)((SaveEntityResponse)flowManager.FlowDataCache.Response).Entity!).Budget);
        }

        [Fact]
        public async Task SaveInvalidSaveDepartment()
        {
            //arrange
            IFlowManager flowManager = serviceProvider!.GetRequiredService<IFlowManager>();
            ISchoolRepository schoolRepository = serviceProvider!.GetRequiredService<ISchoolRepository>();
            var department = (await schoolRepository.GetAsync<DepartmentModel, Department>
            (
                s => s.Name == "Mathematics"
            )).Single();
            department.DepartmentID = 0;
            department.InstructorID = null;
            department.Budget = -1m;
            department.Name = "";
            department.StartDate = default;
            department.EntityState = LogicBuilder.Domain.EntityStateType.Modified;
            flowManager.FlowDataCache.Request = new SaveEntityRequest { Entity = department };

            //act
            System.Diagnostics.Stopwatch stopWatch = System.Diagnostics.Stopwatch.StartNew();
            flowManager.Start("savedepartment");
            stopWatch.Stop();
            this.output.WriteLine("Saving invalid department  = {0}", stopWatch.Elapsed.TotalMilliseconds);

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
