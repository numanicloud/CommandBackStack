using System;
using System.Threading.Tasks;
using Numani.CommandStack.Common;
using Numani.CommandStack.Maybe;

namespace Numani.CommandStack.Pipes;

public sealed class StepPipe<TSource, TMap, TFinal> : ICommandPipe2<TSource, TFinal>
{
    public required Func<TSource, Task<IMaybe<TMap>>> Function { get; init; }
    public required ICommandPipe2<TMap, TFinal> Rest { get; init; }
    
    public async Task<IMaybe<TFinal>> RunAsync(TSource source)
    {
    BackStep:
        var step = await Function.Invoke(source);
        if (step is not Just<TMap> stepJust)
        {
            return Maybe.Maybe.Nothing<TFinal>();
        }

        var final = await Rest.RunAsync(stepJust.Value);
        if (final is not Just<TFinal> finalJust)
        {
            goto BackStep;
        }

        return finalJust;
    }

    public ICommandPipe2<TSource, TNewFinal> WithTail<TNewFinal>(ICommandPipe2<TFinal, TNewFinal> tail)
    {
        return new StepPipe<TSource, TMap, TNewFinal>()
        {
            Function = Function,
            Rest = Rest.WithTail(tail)
        };
    }

    public string ToTreeString(int indent)
    {
        var source = typeof(TSource).ParameterizedName();
        var map = typeof(TMap).ParameterizedName();
        var final = typeof(TFinal).ParameterizedName();
        return
            $"""
            StepPipe ({source} -> {map}) -> {final}
            {Rest.ToTreeString(0)}
            """.Indent(indent);
    }
}