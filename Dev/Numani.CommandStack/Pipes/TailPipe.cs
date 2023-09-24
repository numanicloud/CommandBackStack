using System.Threading.Tasks;
using Numani.CommandStack.Common;
using Numani.CommandStack.Maybe;

namespace Numani.CommandStack.Pipes;

public sealed class TailPipe<TFinal> : ICommandPipe<TFinal, TFinal>
{
    public async Task<IMaybe<TFinal>> RunAsync(TFinal source)
    {
        return source.Just();
    }

    public ICommandPipe<TFinal, TNewFinal> WithTail<TNewFinal>(
        ICommandPipe<TFinal, TNewFinal> tail)
    {
        return tail;
    }

    public string ToTreeString(int indent)
    {
        var final = typeof(TFinal).ParameterizedName();
        return $"Tail {final}".Indent(indent);
    }
}