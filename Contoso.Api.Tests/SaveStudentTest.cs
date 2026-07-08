using Contoso.Domain.Entities;
using LogicBuilder.App.Bsl.Business.Requests;
using LogicBuilder.App.Bsl.Business.Responses;
using LogicBuilder.App.Web.Utils;
using LogicBuilder.App.Web.Utils.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json;

namespace Contoso.Api.Tests
{
    public class SaveStudentTest
    {
        public SaveStudentTest()
        {
            Initialize();
        }

        #region Fields
        private IServiceProvider serviceProvider;
        #endregion Fields

        #region Properties
        private string BaseUrl
        {
            get
            {
                IOptions<UrlOptions> options = serviceProvider.GetRequiredService<IOptions<UrlOptions>>();
                string url = options.Value.BaseBslUrl;
                return url.EndsWith('/') ? url : $"{url}/";
            }
        }
        #endregion Properties

        [Fact]
        public async Task SaveStudent()
        {
            //arrange
            IHttpClientHelper helper = serviceProvider.GetRequiredService<IHttpClientHelper>();

            //act
            List<Task<BaseResponse>> tasks = [];
            for (int i = 0; i < 30; i++)
            {
                tasks.Add
                (
                    helper.PostAsync<BaseResponse>
                    (
                        $"{BaseUrl}api/Student/Save",
                        JsonSerializer.Serialize
                        (
                            new SaveEntityRequest
                            {
                                Entity = new StudentModel
                                {
                                    ID = 1,
                                    FirstName = "Carson",
                                    LastName = "Alexander",
                                    EnrollmentDate = DateTime.SpecifyKind(DateTime.Parse("2010-09-01", CultureInfo.CurrentCulture), DateTimeKind.Unspecified),
                                    EntityState = LogicBuilder.Domain.EntityStateType.Modified,
                                    Enrollments = new HashSet<EnrollmentModel>
                                    {
                                        new() {
                                            EnrollmentID = 1,
                                            CourseID = 1050,
                                            Grade = Contoso.Domain.Entities.Grade.A,
                                            EntityState = LogicBuilder.Domain.EntityStateType.Modified
                                        },
                                        new() {
                                            EnrollmentID = 2,
                                            CourseID = 4022,
                                            Grade = Contoso.Domain.Entities.Grade.C,
                                            EntityState = LogicBuilder.Domain.EntityStateType.Modified
                                        },
                                        new() {
                                            EnrollmentID = 3,
                                            CourseID = 4041,
                                            Grade = Contoso.Domain.Entities.Grade.B,
                                            EntityState = LogicBuilder.Domain.EntityStateType.Modified
                                        }
                                    }
                                }
                            }
                        ),
                        SerializationOptions.Default
                    )
                );

                var results = (await Task.WhenAll(tasks)).ToList();

                results.ForEach(result =>
                {
                    Assert.True(result.Success);
                    Assert.True(result is SaveEntityResponse);
                    Assert.NotNull(((SaveEntityResponse)result).Entity as StudentModel);
                });
            }
        }

        #region Helpers
        [MemberNotNull(nameof(serviceProvider))]
        private void Initialize()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            IServiceCollection services = new ServiceCollection();
            services.AddHttpClient();
            services.AddTransient<IHttpClientHelper, HttpClientHelper>();
            services.Configure<UrlOptions>(configuration);
            serviceProvider = services.BuildServiceProvider();

        }
        #endregion Helpers
    }
}
