using LogicBuilder.Workflow.Activities.Rules;

namespace Contoso.Bsl.Flow.Rules.Interfaces
{
    public interface IRulesSerializer
    {
        RuleSet? DeserializeRuleSet(string ruleSetXmlDefinition);
        RuleSet? DeserializeRuleSetFile(RulesModule module);
        RuleValidation GetValidation(RuleSet ruleSet);
    }
}
