using LogicBuilder.Workflow.Activities.Rules;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Contoso.Bsl.Flow.Rules
{
    internal static class RulesLoaderService
    {
        public static async Task<RulesCache> LoadRules()
        {
            return await LoadRules(new RulesLoader(new RulesSerializer()));
        }

        static async Task<RulesCache> LoadRules(RulesLoader rulesLoader)
        {
            RulesCache cache = new(new ConcurrentDictionary<string, RuleEngine>(), new ConcurrentDictionary<string, string>());

            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(FlowActivity)).Assembly;
            string[] embeddedResources = GetResourceNames(assembly);

            Dictionary<string, string> rules = embeddedResources
                                                .Where(f => f.EndsWith(".module"))
                                                .ToDictionary(f => GetKey(f).ToLowerInvariant());

            Dictionary<string, string> resources = embeddedResources
                                                .Where(f => f.EndsWith(".resources"))
                                                .ToDictionary(f => GetKey(f).ToLowerInvariant());

            await Task.WhenAll
            (
                rules.Keys.Select
                (
                    key => rulesLoader.LoadRulesOnStartUp
                    (
                        new RulesModule
                        (
                            key,
                            GetBytes(resources[key], assembly),
                            GetBytes(rules[key], assembly)
                        ),
                        cache
                    )
                )
            );

            return cache;

            static string GetKey(string fullResourceName)
                => Path.GetExtension(Path.GetFileNameWithoutExtension(fullResourceName))[1..];
            //Gets the full name first: Contoso.Bsl.Flow.Rulesets.savecourse.module
            //Then GetFileNameWithoutExtension returns: Contoso.Bsl.Flow.Rulesets.savecourse
            //Finally Path.GetExtension and the range operator return: savecourse
        }

        private static string[] GetResourceNames(Assembly assembly)
            =>
            [
                .. assembly.GetManifestResourceNames().Where
                (
                    res => res.StartsWith
                    (
                        "Contoso.Bsl.Flow.Rulesets.",
                        System.StringComparison.InvariantCultureIgnoreCase
                    )
                )
            ];

        private static byte[] GetBytes(string file, Assembly assembly)
        {
            using Stream platformStream = assembly.GetManifestResourceStream(file) ?? throw new InvalidOperationException("Rules resources not specified in the assembly.");
            byte[] byteArray = new byte[platformStream.Length];
            platformStream.ReadExactly(byteArray);
            return byteArray;
        }
    }
}
