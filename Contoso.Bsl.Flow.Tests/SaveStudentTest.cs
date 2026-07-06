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
    public class SaveStudentTest : IClassFixture<DatabaseFixture>
    {
        static SaveStudentTest()
        {
            InitializeMapperConfiguration();
        }

        public SaveStudentTest(DatabaseFixture databaseFixture, ITestOutputHelper output)
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
        public async Task SaveStudentRequestWithEnrollments()
        {
            //arrange
            IFlowManager flowManager = serviceProvider!.GetRequiredService<IFlowManager>();
            ISchoolRepository schoolRepository = serviceProvider!.GetRequiredService<ISchoolRepository>();
            var student = (await schoolRepository.GetAsync<StudentModel, Student>
            (
                s => s.FullName == "Carson Alexander",
                selectExpandDefinition: new LogicBuilder.Expressions.Utils.Expansions.SelectExpandDefinition
                (
                    null,
                    [
                        new LogicBuilder.Expressions.Utils.Expansions.SelectExpandItem ("enrollments")
                    ]
                )
            )).Single();
            student.FirstName = "First";
            student.EntityState = LogicBuilder.Domain.EntityStateType.Modified;
            student.Enrollments.ToList().ForEach(enrollment =>
            {
                enrollment.Grade = Domain.Entities.Grade.A;
                enrollment.EntityState = LogicBuilder.Domain.EntityStateType.Modified;
            });
            flowManager.FlowDataCache.Request = new SaveEntityRequest { Entity = student };

            //act
            System.Diagnostics.Stopwatch stopWatch = System.Diagnostics.Stopwatch.StartNew();
            flowManager.Start("savestudent");
            stopWatch.Stop();
            this.output.WriteLine("Saving valid student with enrollments = {0}", stopWatch.Elapsed.TotalMilliseconds);

            //assert
            Assert.True(flowManager.FlowDataCache.Response!.Success);
            Assert.Equal("First", ((StudentModel)((SaveEntityResponse)flowManager.FlowDataCache.Response).Entity!).FirstName);
            Assert.Equal(Domain.Entities.Grade.A, ((StudentModel)((SaveEntityResponse)flowManager.FlowDataCache.Response).Entity!).Enrollments.First().Grade);
        }

        [Fact]
        public async Task SaveValidStudentRequest()
        {
            //arrange
            IFlowManager flowManager = serviceProvider!.GetRequiredService<IFlowManager>();
            ISchoolRepository schoolRepository = serviceProvider!.GetRequiredService<ISchoolRepository>();
            var student = (await schoolRepository.GetAsync<StudentModel, Student>
            (
                s => s.FullName == "Carson Alexander"
            )).Single();
            student.FirstName = "First";
            student.EntityState = LogicBuilder.Domain.EntityStateType.Modified;
            flowManager.FlowDataCache.Request = new SaveEntityRequest { Entity = student };

            //act
            System.Diagnostics.Stopwatch stopWatch = System.Diagnostics.Stopwatch.StartNew();
            flowManager.Start("savestudent");
            stopWatch.Stop();
            this.output.WriteLine("Saving valid student = {0}", stopWatch.Elapsed.TotalMilliseconds);

            //assert
            Assert.True(flowManager.FlowDataCache.Response!.Success);
            Assert.Equal("First", ((StudentModel)((SaveEntityResponse)flowManager.FlowDataCache.Response).Entity!).FirstName);
        }

        [Fact]
        public async Task SaveInvalidStudentRequest()
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
            student.EnrollmentDate = default;
            student.EntityState = LogicBuilder.Domain.EntityStateType.Modified;
            flowManager.FlowDataCache.Request = new SaveEntityRequest { Entity = student };

            //act
            System.Diagnostics.Stopwatch stopWatch = System.Diagnostics.Stopwatch.StartNew();
            flowManager.Start("savestudent");
            stopWatch.Stop();
            this.output.WriteLine("Saving invalid student = {0}", stopWatch.Elapsed.TotalMilliseconds);

            //assert
            Assert.False(flowManager.FlowDataCache.Response!.Success);
            Assert.Equal(3, flowManager.FlowDataCache.Response.ErrorMessages.Count);
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
