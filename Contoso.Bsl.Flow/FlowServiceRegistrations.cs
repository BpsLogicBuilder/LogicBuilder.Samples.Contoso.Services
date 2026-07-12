using Contoso.Bsl.Flow;
using Contoso.Bsl.Flow.Interfaces;
using Contoso.Repositories;
using Contoso.Stores;
using LogicBuilder.App.Bsl.Utils;
using LogicBuilder.App.Bsl.Utils.Interfaces;
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
                .AddFlowFactories()
                .AddBslUtilsServices()
                .AddRulesCacheService()
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
