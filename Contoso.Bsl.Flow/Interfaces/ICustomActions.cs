using LogicBuilder.Attributes;

namespace Contoso.Bsl.Flow.Interfaces
{
    public interface ICustomActions
    {
        [AlsoKnownAs("WriteToLog")]
        void WriteToLog(string message);
    }
}
