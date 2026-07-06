using Contoso.Bsl.Flow.Interfaces;
using LogicBuilder.RulesDirector;

namespace Contoso.Bsl.Flow.Factories
{
    public interface IFlowFactory
    {
        DirectorBase GetDirector(IFlowManager flowManager);
        IFlowActivity GetFlowActivity(IFlowManager flowManager);
    }
}
