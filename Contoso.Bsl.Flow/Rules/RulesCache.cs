using LogicBuilder.RulesDirector;
using LogicBuilder.Workflow.Activities.Rules;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Contoso.Bsl.Flow.Rules
{
    public class RulesCache(ConcurrentDictionary<string, RuleEngine> concurrentRuleEngines, ConcurrentDictionary<string, string> concurrentResourceStrings) : IRulesCache
    {
        private readonly ConcurrentDictionary<string, RuleEngine> concurrentRuleEngines = concurrentRuleEngines;
        private readonly ConcurrentDictionary<string, string> concurrentResourceStrings = concurrentResourceStrings;

        public IDictionary<string, RuleEngine> RuleEngines => concurrentRuleEngines;

        public IDictionary<string, string> ResourceStrings => concurrentResourceStrings;

        public RuleEngine GetRuleEngine(string ruleSet)
        {
            return this.concurrentRuleEngines.TryGetValue(ruleSet.ToLowerInvariant(), out RuleEngine? ruleEngine) ? ruleEngine : throw new InvalidOperationException($"RuleEngine not found for {ruleSet}.");
        }
    }
}
