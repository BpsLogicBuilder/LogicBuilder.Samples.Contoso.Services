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

namespace Contoso.Bsl.Tests
{
    public class SaveInstructorTest
    {
        public SaveInstructorTest()
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
        public async Task SaveInstructor()
        {
            //arrange
            IHttpClientHelper helper = serviceProvider.GetRequiredService<IHttpClientHelper>();

            //act
            List<Task<SaveEntityResponse>> tasks = [];
            for (int i = 0; i < 1; i++)
            {
                tasks.Add
                (
                    helper.PostAsync<SaveEntityResponse>
                    (
                        $"{BaseUrl}api/Instructor/Save",
                        JsonSerializer.Serialize
                        (
                            new SaveEntityRequest
                            {
                                Entity = new InstructorModel
                                {
                                    ID = 3,
                                    FirstName = "Fadi",
                                    LastName = "Fakhouri",
                                    HireDate = DateTime.SpecifyKind(DateTime.Parse("2002-07-07", CultureInfo.InvariantCulture), DateTimeKind.Unspecified),
                                    OfficeAssignment = new OfficeAssignmentModel { Location = "Smith 17", EntityState = LogicBuilder.Domain.EntityStateType.Modified },
                                    Courses =
                                    [
                                        new() { CourseID = 1045, InstructorID = 3, EntityState = LogicBuilder.Domain.EntityStateType.Unchanged }
                                    ],
                                    EntityState = LogicBuilder.Domain.EntityStateType.Modified
                                }
                            }
                        ),
                        SerializationOptions.Default
                    )
                );

                var results = await Task.WhenAll(tasks);

                foreach (var result in results)
                    Assert.True(result.Success);
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
