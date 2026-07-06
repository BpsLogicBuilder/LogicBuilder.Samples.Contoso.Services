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
    public class DeleteStudentTest : IClassFixture<DatabaseFixture>
    {
        static DeleteStudentTest()
        {
            InitializeMapperConfiguration();
        }

        public DeleteStudentTest(DatabaseFixture databaseFixture, ITestOutputHelper output)
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
        public async Task DeleteValidStudentRequest()
        {
            //arrange
            IFlowManager flowManager = serviceProvider!.GetRequiredService<IFlowManager>();
            ISchoolRepository schoolRepository = serviceProvider!.GetRequiredService<ISchoolRepository>();
            var student = (await schoolRepository.GetAsync<StudentModel, Student>
            (
                s => s.FullName == "Carson Alexander"
            )).Single();
            flowManager.FlowDataCache.Request = new DeleteEntityRequest { Entity = student };

            //act
            System.Diagnostics.Stopwatch stopWatch = System.Diagnostics.Stopwatch.StartNew();
            flowManager.Start("deletestudent");
            stopWatch.Stop();
            this.output.WriteLine("Deleting valid student = {0}", stopWatch.Elapsed.TotalMilliseconds);

            student = (await schoolRepository.GetAsync<StudentModel, Student>
            (
                s => s.FullName == "Carson Alexander"
            )).SingleOrDefault();

            //assert
            Assert.True(flowManager.FlowDataCache.Response!.Success);
            Assert.Null(student);
        }

        [Fact]
        public async Task DeleteInvalidStudentRequest()
        {
            //arrange
            IFlowManager flowManager = serviceProvider!.GetRequiredService<IFlowManager>();
            ISchoolRepository schoolRepository = serviceProvider!.GetRequiredService<ISchoolRepository>();
            var student = (await schoolRepository.GetAsync<StudentModel, Student>
            (
                s => s.FullName == "Carson Alexander"
            )).Single();
            student.FirstName = "";
            student.LastName = "";
            flowManager.FlowDataCache.Request = new DeleteEntityRequest { Entity = student };

            //act
            System.Diagnostics.Stopwatch stopWatch = System.Diagnostics.Stopwatch.StartNew();
            flowManager.Start("deletestudent");
            stopWatch.Stop();
            this.output.WriteLine("Deleting invalid student = {0}", stopWatch.Elapsed.TotalMilliseconds);

            student = (await schoolRepository.GetAsync<StudentModel, Student>
            (
                s => s.FullName == "Carson Alexander"
            )).SingleOrDefault();

            //assert
            Assert.False(flowManager.FlowDataCache.Response!.Success);
            Assert.Equal(2, flowManager.FlowDataCache.Response.ErrorMessages.Count);
            Assert.NotNull(student);
        }

        [Fact]
        public async Task DeleteStudentNotFoundRequest()
        {
            //arrange
            IFlowManager flowManager = serviceProvider!.GetRequiredService<IFlowManager>();
            ISchoolRepository schoolRepository = serviceProvider!.GetRequiredService<ISchoolRepository>();
            var student = (await schoolRepository.GetAsync<StudentModel, Student>
            (
                s => s.FullName == "Carson Alexander"
            )).Single();
            student.ID = Int32.MaxValue;
            flowManager.FlowDataCache.Request = new DeleteEntityRequest { Entity = student };

            //act
            System.Diagnostics.Stopwatch stopWatch = System.Diagnostics.Stopwatch.StartNew();
            flowManager.Start("deletestudent");
            stopWatch.Stop();
            this.output.WriteLine("Deleting student not found = {0}", stopWatch.Elapsed.TotalMilliseconds);

            student = (await schoolRepository.GetAsync<StudentModel, Student>(s => s.FullName == "Carson Alexander")).SingleOrDefault();

            //assert
            Assert.False(flowManager.FlowDataCache.Response!.Success);
            Assert.Single(flowManager.FlowDataCache.Response.ErrorMessages);
            Assert.NotNull(student);
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
