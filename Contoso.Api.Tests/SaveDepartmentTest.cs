using Contoso.Data.Entities;
using Contoso.Domain.Entities;
using LogicBuilder.App.Bsl.Business.Requests;
using LogicBuilder.App.Bsl.Business.Responses;
using LogicBuilder.App.Utils.Web;
using LogicBuilder.App.Utils.Web.Interfaces;
using LogicBuilder.Expressions.Utils.ExpressionDescriptors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Contoso.Api.Tests
{
    public class SaveDepartmentTest
    {
        public SaveDepartmentTest()
        {
            Initialize();
        }

        #region Fields
        private IServiceProvider serviceProvider;
        #endregion Fields

        [Fact]
        public async Task SaveDepartment()
        {
            //arrange
            IHttpClientHelper helper = serviceProvider.GetRequiredService<IHttpClientHelper>();
            var departmentResponse = await helper.PostAsync<GetEntityResponse>
            (
                $"{BaseUrl}api/Entity/GetEntity",
                JsonSerializer.Serialize
                (
                    new GetEntityRequest
                    {
                        Filter = GetFilterExpressionDescriptor<DepartmentModel>
                        (
                            GetDepartmentByIdFilterBody(2),
                            "q"
                        ),
                        ModelType = typeof(DepartmentModel).AssemblyQualifiedName,
                        DataType = typeof(Department).AssemblyQualifiedName
                    }
                ),
                SerializationOptions.Default
            );

            DepartmentModel model = (DepartmentModel)departmentResponse.Entity!;
            model.Budget = 100001.00m;
            model.EntityState = LogicBuilder.Domain.EntityStateType.Modified;
            List<Task<SaveEntityResponse>> tasks = [];
            for (int i = 0; i < 1; i++)//department returns a rowversion so can only save one.
            {
                tasks.Add
                (
                    helper.PostAsync<SaveEntityResponse>
                    (
                        $"{BaseUrl}api/Department/Save",
                        JsonSerializer.Serialize
                        (
                            new SaveEntityRequest
                            {
                                Entity = model
                            }
                        ),
                        SerializationOptions.Default
                    )
                );

                var results = (await Task.WhenAll(tasks)).ToList();

                results.ForEach(result => Assert.True(result.Success));
            }
        }

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

        #region Helpers
        private static EqualsBinaryDescriptor GetDepartmentByIdFilterBody(int id)
            => new
            (
                new MemberSelectorDescriptor
                (
                    "DepartmentID",
                    new ParameterDescriptor("q")
                ),
                new ConstantDescriptor(id, typeof(int).AssemblyQualifiedName)
            );

        private static FilterLambdaDescriptor GetFilterExpressionDescriptor<T>(DescriptorBase filterBody, string parameterName = "$it")
            => new
            (
                filterBody,
                typeof(T).AssemblyQualifiedName!,
                parameterName
            );

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
