using LogicBuilder.RulesDirector;
using System.Threading.Tasks;

namespace Contoso.Bsl.Flow.Rules.Interfaces
{
    public interface IRulesLoader
    {
        Task LoadRulesOnStartUp(RulesModule module, IRulesCache cache);
        Task LoadRules(RulesModule module, IRulesCache cache);
    }
}
