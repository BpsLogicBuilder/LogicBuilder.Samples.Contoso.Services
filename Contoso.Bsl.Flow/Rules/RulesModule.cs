namespace Contoso.Bsl.Flow.Rules
{
    public class RulesModule(string name, byte[] resourceSetFile, byte[] ruleSetFile)
    {
        public string Name { get; } = name;
        public byte[] ResourceSetFile { get; } = resourceSetFile;
        public byte[] RuleSetFile { get; } = ruleSetFile;
    }
}
