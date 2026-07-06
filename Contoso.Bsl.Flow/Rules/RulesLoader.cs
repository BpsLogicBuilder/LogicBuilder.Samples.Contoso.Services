using Contoso.Bsl.Flow.Rules.Interfaces;
using LogicBuilder.RulesDirector;
using LogicBuilder.Workflow.Activities.Rules;
using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;

namespace Contoso.Bsl.Flow.Rules
{
    internal class RulesLoader(IRulesSerializer rulesSerializer) : IRulesLoader
    {
        private readonly IRulesSerializer _rulesSerializer = rulesSerializer;

        public Task LoadRules(RulesModule module, IRulesCache cache)
        {
            return Task.Run
            (
                () =>
                {
                    string moduleName = module.Name.ToLowerInvariant();
                    RuleSet ruleSet = _rulesSerializer.DeserializeRuleSetFile(module) ?? throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Properties.Resources.invalidRulesetFormat, moduleName));

                    if (cache.RuleEngines.ContainsKey(moduleName))
                        cache.RuleEngines[moduleName] = new RuleEngine(ruleSet, _rulesSerializer.GetValidation(ruleSet));
                    else
                        cache.RuleEngines.Add(moduleName, new RuleEngine(ruleSet, _rulesSerializer.GetValidation(ruleSet)));

                    using IResourceReader reader = new ResourceReader(new MemoryStream(module.ResourceSetFile));
                    foreach (DictionaryEntry entry in reader.OfType<DictionaryEntry>())
                    {
                        string resourceKey = (string)entry.Key;
                        if (cache.ResourceStrings.ContainsKey(resourceKey))
                            cache.ResourceStrings[resourceKey] = (string)(entry.Value ?? "");
                        else
                            cache.ResourceStrings.Add(resourceKey, (string)(entry.Value ?? ""));
                    }
                }
            );
        }

        public Task LoadRulesOnStartUp(RulesModule module, IRulesCache cache)
        {
            return Task.Run
            (
                () =>
                {
                    string moduleName = module.Name.ToLowerInvariant();
                    RuleSet ruleSet = _rulesSerializer.DeserializeRuleSetFile(module) ?? throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Properties.Resources.invalidRulesetFormat, moduleName));
                    cache.RuleEngines.Add(moduleName, new RuleEngine(ruleSet, _rulesSerializer.GetValidation(ruleSet)));

                    using IResourceReader reader = new ResourceReader(new MemoryStream(module.ResourceSetFile));
                    foreach (DictionaryEntry entry in reader.OfType<DictionaryEntry>())
                        cache.ResourceStrings.Add((string)entry.Key, (string)(entry.Value ?? ""));
                }
            );
        }
    }
}
