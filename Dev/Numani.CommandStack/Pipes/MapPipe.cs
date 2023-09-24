using System;
using System.Threading.Tasks;
using Numani.CommandStack.Common;
using Numani.CommandStack.Maybe;

namespace Numani.CommandStack.Pipes;

public sealed class MapPipe<TSource, TMap, TFinal> : ICommandPipe<TSource, TFinal>
{
    public required Func<TSource, IMaybe<TMap>> Mapper { get; init; }
    public required ICommandPipe<TMap, TFinal> Rest { get; init; }

    public async Task<IMaybe<TFinal>> RunAsync(TSource source)
    {
        var map = Mapper.Invoke(source);
        if (map is not Just<TMap> just)
        {
            return Maybe.Maybe.Nothing<TFinal>();
        }

        return await Rest.RunAsync(just.Value);
    }

    public ICommandPipe<TSource, TNewFinal> WithTail<TNewFinal>(
        ICommandPipe<TFinal, TNewFinal> tail)
    {
        return new MapPipe<TSource, TMap, TNewFinal>()
        {
            Mapper = Mapper,
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
            Map ({source} -> {map}) -> {final}
            {Rest.ToTreeString(0)}
            """.Indent(indent);
    }
}