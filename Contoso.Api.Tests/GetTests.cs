using Contoso.Data.Entities;
using Contoso.Domain.Entities;
using LogicBuilder.App.Bsl.Business.Requests;
using LogicBuilder.App.Bsl.Business.Responses;
using LogicBuilder.App.Web.Utils;
using LogicBuilder.App.Web.Utils.Interfaces;
using LogicBuilder.Expressions.Utils.ExpansionDescriptors;
using LogicBuilder.Expressions.Utils.ExpressionDescriptors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Contoso.Api.Tests
{
    public class GetTests
    {
        public GetTests()
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
        public async Task Select_Credits_AsCredits_From_Lookups_Table_In_Descending_Order_As_LookUpsModel()
        {
            //arrange
            IHttpClientHelper helper = serviceProvider.GetRequiredService<IHttpClientHelper>();
            var selectorLambdaDescriptor = GetExpressionDescriptor<IQueryable<LookUpsModel>, IEnumerable<object>>
            (
                GetCreditsBodyAsCreditsLookupsModel(),
                "$it"
            );

            //act
            var result = await helper.PostAsync<GetObjectListResponse>
            (
                $"{BaseUrl}api/AnonymousTypeList/GetList",
                JsonSerializer.Serialize
                (
                    new GetObjectListRequest
                    {
                        Selector = selectorLambdaDescriptor,
                        ModelType = typeof(LookUpsModel).AssemblyQualifiedName,
                        DataType = typeof(LookUps).AssemblyQualifiedName
                    }
                ),
                SerializationOptions.Default
            );

            //assert
            Assert.True(result.List.Any());
        }

        [Fact]
        public async Task Select_CourseIds_AsCourseIds_From_Course_Table_In_Ascending_Order()
        {
            //arrange
            IHttpClientHelper helper = serviceProvider.GetRequiredService<IHttpClientHelper>();
            var selectorLambdaOperatorDescriptor = GetExpressionDescriptor<IQueryable<CourseModel>, IEnumerable<object>>
            (
                GetCourseBodyAsCourseIDFromCourseModel(),
                "$it"
            );

            //act
            var result = await helper.PostAsync<GetObjectListResponse>
            (
                $"{BaseUrl}api/AnonymousTypeList/GetList",
                JsonSerializer.Serialize
                (
                    new GetObjectListRequest
                    {
                        Selector = selectorLambdaOperatorDescriptor,
                        ModelType = typeof(CourseModel).AssemblyQualifiedName,
                        DataType = typeof(Course).AssemblyQualifiedName
                    }
                ),
                SerializationOptions.Default
            );

            //asser
            Assert.True(result.List.Any());
        }

        [Fact]
        public async Task GetDropDownListRequest_AdministratorLookup()
        {
            //arrange
            IHttpClientHelper helper = serviceProvider.GetRequiredService<IHttpClientHelper>();
            var selectorLambdaOperatorDescriptor = GetExpressionDescriptor<IQueryable<InstructorModel>, IQueryable<InstructorModel>>
            (
                GetBodyForAdministratorLookup(),
                "$it"
            );

            //act
            var result = await helper.PostAsync<GetListResponse>
            (
                $"{BaseUrl}api/List/GetList",
                JsonSerializer.Serialize
                (
                    new GetTypedListRequest
                    {
                        Selector = selectorLambdaOperatorDescriptor,
                        ModelType = typeof(InstructorModel).AssemblyQualifiedName,
                        DataType = typeof(Instructor).AssemblyQualifiedName,
                        ModelReturnType = typeof(IQueryable<InstructorModel>).AssemblyQualifiedName,
                        DataReturnType = typeof(IQueryable<Instructor>).AssemblyQualifiedName
                    }
                ),
                SerializationOptions.Default
            );

            Assert.True(result.List.Any());
        }

        [Fact]
        public async Task GetDropDownListRequest_As_LookUpsModel()
        {
            //arrange
            IHttpClientHelper helper = serviceProvider.GetRequiredService<IHttpClientHelper>();
            var selectorLambdaOperatorDescriptor = GetExpressionDescriptor<IQueryable<LookUpsModel>, IEnumerable<LookUpsModel>>
            (
                GetBodyForLookupsModel(),
                "q"
            );

            //act
            var result = await helper.PostAsync<GetListResponse>
            (
                $"{BaseUrl}api/List/GetList",
                JsonSerializer.Serialize
                (
                    new GetTypedListRequest
                    {
                        Selector = selectorLambdaOperatorDescriptor,
                        ModelType = typeof(LookUpsModel).AssemblyQualifiedName,
                        DataType = typeof(LookUps).AssemblyQualifiedName,
                        ModelReturnType = typeof(IEnumerable<LookUpsModel>).AssemblyQualifiedName,
                        DataReturnType = typeof(IEnumerable<LookUps>).AssemblyQualifiedName
                    }
                ),
                SerializationOptions.Default
            );

            Assert.True(result.List.Any());
        }

        [Fact]
        public async Task GetDropDownListRequest_As_LookUpsModel_Using_Object_ReturnType()
        {
            //arrange
            IHttpClientHelper helper = serviceProvider.GetRequiredService<IHttpClientHelper>();
            var selectorLambdaOperatorDescriptor = GetExpressionDescriptor<IQueryable<LookUpsModel>, IEnumerable<object>>
            (
                GetBodyForLookupsModelAsAnonymousTypeList(),
                "q"
            );

            //act
            var result = await helper.PostAsync<GetObjectListResponse>
            (
                $"{BaseUrl}api/AnonymousTypeList/GetList",
                JsonSerializer.Serialize
                (
                    new GetObjectListRequest
                    {
                        Selector = selectorLambdaOperatorDescriptor,
                        ModelType = typeof(LookUpsModel).AssemblyQualifiedName,
                        DataType = typeof(LookUps).AssemblyQualifiedName
                    }
                ),
                SerializationOptions.Default
            );

            Assert.True(result.List.Any());
        }

        [Fact]
        public async Task GetDropDownListRequest_As_DepartmentModel_Using_Object_ReturnType()
        {
            //arrange
            IHttpClientHelper helper = serviceProvider.GetRequiredService<IHttpClientHelper>();
            var selectorLambdaOperatorDescriptor = GetExpressionDescriptor<IQueryable<DepartmentModel>, IEnumerable<DepartmentModel>>
            (
                GetDepartmentsBodyForDepartmentModelType(),
                "q"
            );

            //act
            var result = await helper.PostAsync<GetListResponse>
            (
                $"{BaseUrl}api/List/GetList",
                JsonSerializer.Serialize
                (
                    new GetTypedListRequest
                    {
                        Selector = selectorLambdaOperatorDescriptor,
                        ModelType = typeof(DepartmentModel).AssemblyQualifiedName,
                        DataType = typeof(Department).AssemblyQualifiedName,
                        ModelReturnType = typeof(IEnumerable<DepartmentModel>).AssemblyQualifiedName,
                        DataReturnType = typeof(IEnumerable<Department>).AssemblyQualifiedName
                    }
                ),
                SerializationOptions.Default
            );

            Assert.True(result.List.Any());
        }

        [Fact]
        public async Task GetListRequest_As_CourseModel()
        {
            //arrange
            IHttpClientHelper helper = serviceProvider.GetRequiredService<IHttpClientHelper>();
            var selectorLambdaOperatorDescriptor = GetExpressionDescriptor<IQueryable<CourseModel>, IEnumerable<CourseModel>>
            (
                GetCoursesBodyForCourseModelType(),
                "q"
            );

            //act
            var result = await helper.PostAsync<GetListResponse>
            (
                $"{BaseUrl}api/List/GetList",
                JsonSerializer.Serialize
                (
                    new GetTypedListRequest
                    {
                        Selector = selectorLambdaOperatorDescriptor,
                        ModelType = typeof(CourseModel).AssemblyQualifiedName,
                        DataType = typeof(Course).AssemblyQualifiedName,
                        ModelReturnType = typeof(IEnumerable<CourseModel>).AssemblyQualifiedName,
                        DataReturnType = typeof(IEnumerable<Course>).AssemblyQualifiedName
                    }
                ),
                SerializationOptions.Default
            );

            Assert.True(result.List.Any());
        }

        [Fact]
        public async Task GetEntityRequest_As_DeopartmentModel()
        {
            //arrange
            IHttpClientHelper helper = serviceProvider.GetRequiredService<IHttpClientHelper>();

            //act
            var result = await helper.PostAsync<GetEntityResponse>
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
                        SelectExpandDefinition = new SelectExpandDefinitionDescriptor
                        {
                            ExpandedItems =
                            [
                                new SelectExpandItemDescriptor("Courses")
                            ]
                        },
                        ModelType = typeof(DepartmentModel).AssemblyQualifiedName,
                        DataType = typeof(Department).AssemblyQualifiedName
                    }
                ),
                SerializationOptions.Default
            );

            Assert.NotNull(result);
            Assert.NotNull(result.Entity);
        }

        [Fact]
        public async Task GetEntityRequest_As_DeopartmentModel_FromObjectConstant()
        {
            //arrange
            IHttpClientHelper helper = serviceProvider.GetRequiredService<IHttpClientHelper>();

            //act
            var result = await helper.PostAsync<GetEntityResponse>
            (
                $"{BaseUrl}api/Entity/GetEntity",
                JsonSerializer.Serialize
                (
                    new GetEntityRequest
                    {
                        Filter = GetFilterExpressionDescriptor<DepartmentModel>
                        (
                            GetDepartmentByIdFilterBodyFromObjectConstant(new DepartmentModel { DepartmentID = 2 }),
                            "q"
                        ),
                        SelectExpandDefinition = new SelectExpandDefinitionDescriptor
                        {
                            ExpandedItems =
                            [
                                new SelectExpandItemDescriptor("Courses")
                            ]
                        },
                        ModelType = typeof(DepartmentModel).AssemblyQualifiedName,
                        DataType = typeof(Department).AssemblyQualifiedName
                    }
                ),
                SerializationOptions.Default
            );

            Assert.NotNull(result);
            Assert.NotNull(result.Entity);
        }

        [Fact]
        public async Task GetAboutListRequest_StudentEnrollmentCountByEnrollmentDate_As_LookUpsModel()
        {
            //arrange
            IHttpClientHelper helper = serviceProvider.GetRequiredService<IHttpClientHelper>();
            var selectorLambdaOperatorDescriptor = GetExpressionDescriptor<IQueryable<StudentModel>, IEnumerable<LookUpsModel>>
            (
                GetAboutBody_StudentEnrollmentCountByEnrollmentDate(),
                "q"
            );

            //act
            var result = await helper.PostAsync<GetListResponse>
            (
                $"{BaseUrl}api/List/GetList",
                JsonSerializer.Serialize
                (
                    new GetTypedListRequest
                    {
                        Selector = selectorLambdaOperatorDescriptor,
                        ModelType = typeof(StudentModel).AssemblyQualifiedName,
                        DataType = typeof(Student).AssemblyQualifiedName,
                        ModelReturnType = typeof(IEnumerable<LookUpsModel>).AssemblyQualifiedName,
                        DataReturnType = typeof(IEnumerable<LookUps>).AssemblyQualifiedName
                    }
                ),
                SerializationOptions.Default
            );

            Assert.True(result.List.Any());
        }

        #region Helpers
        private static SelectDescriptor GetAboutBody_StudentEnrollmentCountByEnrollmentDate()
            => new
            (
                new OrderByDescriptor
                (
                    new GroupByDescriptor
                    (
                        new ParameterDescriptor("q"),
                        new MemberSelectorDescriptor
                        (
                            "EnrollmentDate",
                            new ParameterDescriptor("item")
                        ),
                        "item"
                    ),
                    new MemberSelectorDescriptor
                    (
                        "Key",
                        new ParameterDescriptor("group")
                    ),
                    LogicBuilder.Expressions.Utils.Strutures.ListSortDirection.Descending,
                    "group"
                ),
                new MemberInitDescriptor
                (
                    new Dictionary<string, DescriptorBase>
                    {
                        ["DateTimeValue"] = new MemberSelectorDescriptor
                        (
                            "Key",
                            new ParameterDescriptor("sel")
                        ),
                        ["NumericValue"] = new ConvertDescriptor
                        (
                            new CountDescriptor
                            (
                                new AsQueryableDescriptor
                                (
                                    new ParameterDescriptor("sel")
                                )
                            ),
                            typeof(double?).AssemblyQualifiedName!
                        )
                    },
                    typeof(LookUpsModel).AssemblyQualifiedName
                ),
                "sel"
            );

        private static SelectDescriptor GetBodyForAdministratorLookup()
            => new
            (
                new OrderByDescriptor
                (
                    new ParameterDescriptor("$it"),
                    new MemberSelectorDescriptor
                    (
                        "FullName",
                        new ParameterDescriptor("d")
                    ),
                    LogicBuilder.Expressions.Utils.Strutures.ListSortDirection.Ascending,
                    "d"
                ),
                new MemberInitDescriptor
                (
                    new Dictionary<string, DescriptorBase>
                    {
                        ["ID"] = new MemberSelectorDescriptor
                        (
                            "ID",
                            new ParameterDescriptor("s")
                        ),
                        ["FirstName"] = new MemberSelectorDescriptor
                        (
                            "FirstName",
                            new ParameterDescriptor("s")
                        ),
                        ["LastName"] = new MemberSelectorDescriptor
                        (
                            "LastName",
                            new ParameterDescriptor("s")
                        ),
                        ["FullName"] = new MemberSelectorDescriptor
                        (
                            "FullName",
                            new ParameterDescriptor("s")
                        )
                    },
                    typeof(InstructorModel).AssemblyQualifiedName
                ),
                "s"
            );

        private static SelectDescriptor GetBodyForLookupsModel()
            => new
            (
                new OrderByDescriptor
                (
                    new WhereDescriptor
                    (
                        new ParameterDescriptor("q"),
                        new EqualsBinaryDescriptor
                        (
                            new MemberSelectorDescriptor
                            (
                                "ListName",
                                new ParameterDescriptor("l")
                            ),
                            new ConstantDescriptor
                            (
                                "Credits",
                                typeof(string).AssemblyQualifiedName
                            )
                        ),
                        "l"
                    ),
                    new MemberSelectorDescriptor
                    (
                        "NumericValue",
                        new ParameterDescriptor("l")
                    ),
                    LogicBuilder.Expressions.Utils.Strutures.ListSortDirection.Descending,
                    "l"
                ),
                new MemberInitDescriptor
                (
                    new Dictionary<string, DescriptorBase>
                    {
                        ["NumericValue"] = new MemberSelectorDescriptor
                        (
                            "NumericValue",
                            new ParameterDescriptor("l")
                        ),
                        ["Text"] = new MemberSelectorDescriptor
                        (
                            "Text",
                            new ParameterDescriptor("l")
                        )
                    },
                    typeof(LookUpsModel).AssemblyQualifiedName
                ),
                "l"
            );

        private static SelectDescriptor GetBodyForLookupsModelAsAnonymousTypeList()
            => new
            (
                new OrderByDescriptor
                (
                    new WhereDescriptor
                    (
                        new ParameterDescriptor("q"),
                        new EqualsBinaryDescriptor
                        (
                            new MemberSelectorDescriptor
                            (
                                "ListName",
                                new ParameterDescriptor("l")
                            ),
                            new ConstantDescriptor
                            (
                                "Credits",
                                typeof(string).AssemblyQualifiedName
                            )
                        ),
                        "l"
                    ),
                    new MemberSelectorDescriptor
                    (
                        "NumericValue",
                        new ParameterDescriptor("l")
                    ),
                    LogicBuilder.Expressions.Utils.Strutures.ListSortDirection.Descending,
                    "l"
                ),
                new MemberInitDescriptor
                (
                    new Dictionary<string, DescriptorBase>
                    {
                        ["NumericValue"] = new MemberSelectorDescriptor
                        (
                            "NumericValue",
                            new ParameterDescriptor("l")
                        ),
                        ["Text"] = new MemberSelectorDescriptor
                        (
                            "Text",
                            new ParameterDescriptor("l")
                        )
                    }
                ),
                "l"
            );

        private static SelectDescriptor GetCourseBodyAsCourseIDFromCourseModel()
            => new
            (
                new OrderByDescriptor
                (
                    new ParameterDescriptor("$it"),
                    new MemberSelectorDescriptor
                    (
                        "CourseID",
                        new ParameterDescriptor("o")
                    ),
                    LogicBuilder.Expressions.Utils.Strutures.ListSortDirection.Ascending,
                    "o"
                ),
                new MemberInitDescriptor
                (
                    new Dictionary<string, DescriptorBase>
                    {
                        ["courseID"] = new MemberSelectorDescriptor
                        (
                            "CourseID",
                            new ParameterDescriptor("s")
                        )
                    }
                ),
                "s"
            );

        private static OrderByDescriptor GetCoursesBodyForCourseModelType()
            => new
            (
                new ParameterDescriptor("q"),
                new MemberSelectorDescriptor
                (
                    "Title",
                    new ParameterDescriptor("d")
                ),
                LogicBuilder.Expressions.Utils.Strutures.ListSortDirection.Ascending,
                "d"
            );

        private static SelectDescriptor GetCreditsBodyAsCreditsLookupsModel()
            => new
            (
                new OrderByDescriptor
                (
                    new WhereDescriptor
                    (
                        new ParameterDescriptor("$it"),
                        new EqualsBinaryDescriptor
                        (
                            new MemberSelectorDescriptor
                            (
                                "ListName",
                                new ParameterDescriptor("w")
                            ),
                            new ConstantDescriptor
                            (
                                "Credits",
                                typeof(string).AssemblyQualifiedName
                            )
                        ),
                        "w"
                    ),
                    new MemberSelectorDescriptor
                    (
                        "NumericValue",
                        new ParameterDescriptor("o")
                    ),
                    LogicBuilder.Expressions.Utils.Strutures.ListSortDirection.Descending,
                    "o"
                ),
                new MemberInitDescriptor
                (
                    new Dictionary<string, DescriptorBase>
                    {
                        ["credits"] = new MemberSelectorDescriptor
                        (
                            "NumericValue",
                            new ParameterDescriptor("s")
                        )
                    }
                ),
                "s"
            );

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

        private static EqualsBinaryDescriptor GetDepartmentByIdFilterBodyFromObjectConstant(DepartmentModel department)
            => new
            (
                new MemberSelectorDescriptor
                (
                    "DepartmentID",
                    new ParameterDescriptor("q")
                ),
                new MemberSelectorDescriptor
                (
                    "DepartmentID",
                    new ConstantDescriptor(department, typeof(DepartmentModel).AssemblyQualifiedName)
                )
            );

        private static SelectDescriptor GetDepartmentsBodyForDepartmentModelType()
            => new
            (
                new OrderByDescriptor
                (
                    new ParameterDescriptor("q"),
                    new MemberSelectorDescriptor
                    (
                        "Name",
                        new ParameterDescriptor("d")
                    ),
                    LogicBuilder.Expressions.Utils.Strutures.ListSortDirection.Ascending,
                    "d"
                ),
                new MemberInitDescriptor
                (
                    new Dictionary<string, DescriptorBase>
                    {
                        ["DepartmentID"] = new MemberSelectorDescriptor
                        (
                            "DepartmentID",
                            new ParameterDescriptor("d")
                        ),
                        ["Name"] = new MemberSelectorDescriptor
                        (
                            "Name",
                            new ParameterDescriptor("d")
                        )
                    },
                    typeof(DepartmentModel).AssemblyQualifiedName
                ),
                "d"
            );

        private static SelectorLambdaDescriptor GetExpressionDescriptor<T, TResult>(DescriptorBase selectorBody, string parameterName = "$it")
            => new
            (
                selectorBody,
                typeof(T).AssemblyQualifiedName!,
                parameterName,
                typeof(TResult).AssemblyQualifiedName
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
