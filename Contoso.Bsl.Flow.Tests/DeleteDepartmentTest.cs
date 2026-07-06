using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using Contoso.Bsl.Flow.Interfaces;
using Contoso.BSL.AutoMapperProfiles;
using Contoso.Contexts;
using Contoso.Data.Entities;
using Contoso.Domain.Entities;
using Contoso.Repositories;
using LogicBuilder.App.Bsl.Business.Requests;
using LogicBuilder.EntityFrameworkCore.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Contoso.Bsl.Flow.Tests
{
    public class DeleteDepartmentTest : IClassFixture<DatabaseFixture>
    {
        static DeleteDepartmentTest()
        {
            InitializeMapperConfiguration();
        }

        public DeleteDepartmentTest(DatabaseFixture databaseFixture, ITestOutputHelper output)
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
        public async Task DeleteValidDepartmentRequest()
        {
            //arrange
            IFlowManager flowManager = serviceProvider!.GetRequiredService<IFlowManager>();
            ISchoolRepository schoolRepository = serviceProvider!.GetRequiredService<ISchoolRepository>();
            var department = (await schoolRepository.GetAsync<DepartmentModel, Department>
            (
                s => s.Name == "Mathematics"
            )).Single();
            flowManager.FlowDataCache.Request = new DeleteEntityRequest { Entity = department };

            //act
            System.Diagnostics.Stopwatch stopWatch = System.Diagnostics.Stopwatch.StartNew();
            flowManager.Start("deletedepartment");
            stopWatch.Stop();
            this.output.WriteLine("Deleting valid department = {0}", stopWatch.Elapsed.TotalMilliseconds);

            department = (await schoolRepository.GetAsync<DepartmentModel, Department>
            (
                s => s.Name == "Mathematics"
            )).SingleOrDefault();

            //assert
            Assert.True(flowManager.FlowDataCache.Response!.Success);
            Assert.Null(department);
        }

        [Fact]
        public async Task DeleteInvalidDepartmentRequest()
        {
            //arrange
            IFlowManager flowManager = serviceProvider!.GetRequiredService<IFlowManager>();
            ISchoolRepository schoolRepository = serviceProvider!.GetRequiredService<ISchoolRepository>();
            var department = (await schoolRepository.GetAsync<DepartmentModel, Department>
            (
                s => s.Name == "Mathematics"
            )).Single();
            department.Name = "";
            flowManager.FlowDataCache.Request = new DeleteEntityRequest { Entity = department };

            //act
            System.Diagnostics.Stopwatch stopWatch = System.Diagnostics.Stopwatch.StartNew();
            flowManager.Start("deletedepartment");
            stopWatch.Stop();
            this.output.WriteLine("Deleting invalid department = {0}", stopWatch.Elapsed.TotalMilliseconds);

            department = (await schoolRepository.GetAsync<DepartmentModel, Department>
            (
                s => s.Name == "Mathematics"
            )).SingleOrDefault();

            //assert
            Assert.False(flowManager.FlowDataCache.Response!.Success);
            Assert.Single(flowManager.FlowDataCache.Response.ErrorMessages);
            Assert.NotNull(department);
        }

        [Fact]
        public async Task DeleteDepartmentNotFoundRequest()
        {
            //arrange
            IFlowManager flowManager = serviceProvider!.GetRequiredService<IFlowManager>();
            ISchoolRepository schoolRepository = serviceProvider!.GetRequiredService<ISchoolRepository>();
            var department = (await schoolRepository.GetAsync<DepartmentModel, Department>
            (
                s => s.Name == "Mathematics"
            )).Single();
            department.DepartmentID = Int32.MaxValue;
            flowManager.FlowDataCache.Request = new DeleteEntityRequest { Entity = department };

            //act
            System.Diagnostics.Stopwatch stopWatch = System.Diagnostics.Stopwatch.StartNew();
            flowManager.Start("deletedepartment");
            stopWatch.Stop();
            this.output.WriteLine("Deleting department not found = {0}", stopWatch.Elapsed.TotalMilliseconds);

            department = (await schoolRepository.GetAsync<DepartmentModel, Department>
            (
                s => s.Name == "Mathematics"
            )).SingleOrDefault();

            //assert
            Assert.False(flowManager.FlowDataCache.Response!.Success);
            Assert.Single(flowManager.FlowDataCache.Response.ErrorMessages);
            Assert.NotNull(department);
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
