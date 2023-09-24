using System;
using System.Threading.Tasks;
using Numani.CommandStack.Common;
using Numani.CommandStack.Maybe;

namespace Numani.CommandStack.Pipes2;

public sealed class DynamicPipe<TSource, TMap, TFinal> : ICommandPipe2<TSource, TFinal>
{
    public required Func<TSource, ICommandPipe2<TSource, TMap>> Generator { get; init; }
    public required ICommandPipe2<TMap, TFinal> Rest { get; init; }

    public async Task<IMaybe<TFinal>> RunAsync(TSource source)
    {
    Back:
        var main = await Generator.Invoke(source).RunAsync(source);
        if (main is not Just<TMap> map)
        {
            return Maybe.Maybe.Nothing<TFinal>();
        }

        var final = await Rest.RunAsync(map.Value);
        if (final is not Just<TFinal> just)
        {
            goto Back;
        }

        return just;
    }

    public ICommandPipe2<TSource, TNewFinal> WithTail<TNewFinal>(
        ICommandPipe2<TFinal, TNewFinal> tail)
    {
        return new DynamicPipe<TSource, TMap, TNewFinal>()
        {
            Generator = Generator,
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
            DynamicPipe ({source} -> {map}) -> {final}
            {Rest.ToTreeString(0)}
            """.Indent(indent);
    }
}