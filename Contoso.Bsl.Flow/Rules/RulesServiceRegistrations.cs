using Contoso.Bsl.Flow.Rules;
using LogicBuilder.RulesDirector;
using System;
using System.Collections.Generic;
using System.Text;

#pragma warning disable IDE0130 //Microsoft recommended namespace for service registrations
namespace Microsoft.Extensions.DependencyInjection
#pragma warning restore IDE0130
{
    public static class RulesServiceRegistrations
    {
        public static IServiceCollection AddRulesCacheService(this IServiceCollection services)
        {
            IRulesCache rulesCache = RulesLoaderService.LoadRules().GetAwaiter().GetResult();
            return services
                .AddSingleton(sp => rulesCache);
        }
    }
}
