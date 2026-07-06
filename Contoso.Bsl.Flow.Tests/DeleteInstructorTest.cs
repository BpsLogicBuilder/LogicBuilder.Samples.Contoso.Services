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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contoso.Bsl.Flow.Tests
{
    public class DeleteInstructorTest : IClassFixture<DatabaseFixture>
    {
        static DeleteInstructorTest()
        {
            InitializeMapperConfiguration();
        }

        public DeleteInstructorTest(DatabaseFixture databaseFixture, ITestOutputHelper output)
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
        public async Task DeleteValidInstructorRequest()
        {
            //arrange
            IFlowManager flowManager = serviceProvider!.GetRequiredService<IFlowManager>();
            ISchoolRepository schoolRepository = serviceProvider!.GetRequiredService<ISchoolRepository>();
            var instructor = (await schoolRepository.GetAsync<InstructorModel, Instructor>
            (
                s => s.FullName == "Candace Kapoor"
            )).Single();
            flowManager.FlowDataCache.Request = new DeleteEntityRequest { Entity = instructor };

            //act
            System.Diagnostics.Stopwatch stopWatch = System.Diagnostics.Stopwatch.StartNew();
            flowManager.Start("deleteinstructor");
            stopWatch.Stop();
            this.output.WriteLine("Deleting valid instructor = {0}", stopWatch.Elapsed.TotalMilliseconds);

            instructor = (await schoolRepository.GetAsync<InstructorModel, Instructor>
            (
                s => s.FullName == "Candace Kapoor"
            )).SingleOrDefault();

            //assert
            Assert.True(flowManager.FlowDataCache.Response!.Success);
            Assert.Null(instructor);
        }

        [Fact]
        public async Task DeleteInvalidInstructorRequest()
        {
            //arrange
            IFlowManager flowManager = serviceProvider!.GetRequiredService<IFlowManager>();
            ISchoolRepository schoolRepository = serviceProvider!.GetRequiredService<ISchoolRepository>();
            var instructor = (await schoolRepository.GetAsync<InstructorModel, Instructor>(s => s.FullName == "Candace Kapoor")).Single();
            instructor.FirstName = "";
            instructor.LastName = "";
            flowManager.FlowDataCache.Request = new DeleteEntityRequest { Entity = instructor };

            //act
            System.Diagnostics.Stopwatch stopWatch = System.Diagnostics.Stopwatch.StartNew();
            flowManager.Start("deleteinstructor");
            stopWatch.Stop();
            this.output.WriteLine("Deleting invalid instructor = {0}", stopWatch.Elapsed.TotalMilliseconds);

            instructor = (await schoolRepository.GetAsync<InstructorModel, Instructor>
            (
                s => s.FullName == "Candace Kapoor"
            )).SingleOrDefault();

            //assert
            Assert.False(flowManager.FlowDataCache.Response!.Success);
            Assert.Equal(2, flowManager.FlowDataCache.Response.ErrorMessages.Count);
            Assert.NotNull(instructor);
        }

        [Fact]
        public async Task DeleteInstructorNotFoundRequest()
        {
            //arrange
            IFlowManager flowManager = serviceProvider!.GetRequiredService<IFlowManager>();
            ISchoolRepository schoolRepository = serviceProvider!.GetRequiredService<ISchoolRepository>();
            var instructor = (await schoolRepository.GetAsync<InstructorModel, Instructor>
            (
                s => s.FullName == "Candace Kapoor"
            )).Single();
            instructor.ID = Int32.MaxValue;
            flowManager.FlowDataCache.Request = new DeleteEntityRequest { Entity = instructor };

            //act
            System.Diagnostics.Stopwatch stopWatch = System.Diagnostics.Stopwatch.StartNew();
            flowManager.Start("deleteinstructor");
            stopWatch.Stop();
            this.output.WriteLine("Deleting instructor not found = {0}", stopWatch.Elapsed.TotalMilliseconds);

            instructor = (await schoolRepository.GetAsync<InstructorModel, Instructor>
            (
                s => s.FullName == "Candace Kapoor"
            )).SingleOrDefault();

            //assert
            Assert.False(flowManager.FlowDataCache.Response!.Success);
            Assert.Single(flowManager.FlowDataCache.Response.ErrorMessages);
            Assert.NotNull(instructor);
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
