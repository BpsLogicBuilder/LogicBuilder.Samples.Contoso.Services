using Contoso.Bsl.Flow;
using Contoso.Bsl.Flow.Interfaces;
using Contoso.Repositories;
using Contoso.Stores;
using LogicBuilder.App.Utils.Rules;
using LogicBuilder.EntityFrameworkCore.Repositories;
using LogicBuilder.RulesDirector;

#pragma warning disable IDE0130 //Microsoft recommended namespace for service registrations
namespace Microsoft.Extensions.DependencyInjection
#pragma warning restore IDE0130
{
    public static class FlowServiceRegistrations
    {
        public static IServiceCollection AddContosoBslFlowServices(this IServiceCollection services)
        {
            return services
                .AddAppUtilsServices()
                .AddHttpClient()
                .AddFlowFactories()
                .AddBslUtilsServices()
                .AddRulesCacheService
                (
                    new RulesLoaderRequest
                    (
                        "Contoso.Bsl.Flow.Rulesets",
                        typeof(FlowActivity),
                        [
                            typeof(LogicBuilder.App.Utils.Interfaces.ITypeHelper).Assembly,
                            typeof(LogicBuilder.Forms.Parameters.Expansions.SelectExpandDefinitionParameters).Assembly,
                            typeof(Contoso.Domain.Entities.StudentModel).Assembly,
                            typeof(Contoso.Data.Entities.Course).Assembly,
                            typeof(DirectorBase).Assembly,
                            typeof(string).Assembly
                        ]
                    )
                )
                .AddTransient<ISchoolStore, SchoolStore>()
                .AddTransient<IContextRepository, SchoolRepository>()
                .AddTransient<ISchoolRepository, SchoolRepository>()
                .AddTransient<ICustomActions, CustomActions>()
                .AddTransient<IFlowManager, FlowManager>()
                .AddScoped<IFlowDataCache, FlowDataCache>()
                .AddScoped<Progress>();
        }
    }
}
