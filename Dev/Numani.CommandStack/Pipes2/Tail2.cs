using System.Threading.Tasks;
using Numani.CommandStack.Maybe;

namespace Numani.CommandStack.Pipes2;

public sealed class Tail2<TFinal> : ICommandPipe2<TFinal, TFinal>
{
    public async Task<IMaybe<TFinal>> RunAsync(TFinal source)
    {
        return source.Just();
    }

    public ICommandPipe2<TFinal, TNewFinal> WithTail<TNewFinal>(
        ICommandPipe2<TFinal, TNewFinal> tail)
    {
        return tail;
    }

    public string ToTreeString(int indent)
    {
        var final = typeof(TFinal).ParameterizedName();
        return $"Tail {final}".Indent(indent);
    }
}