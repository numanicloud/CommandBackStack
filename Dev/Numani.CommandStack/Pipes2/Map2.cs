using System;
using System.Threading.Tasks;
using Numani.CommandStack.Maybe;

namespace Numani.CommandStack.Pipes2;

public sealed class Map2<TSource, TMap, TFinal> : ICommandPipe2<TSource, TFinal>
{
    public required Func<TSource, IMaybe<TMap>> Mapper { get; init; }
    public required ICommandPipe2<TMap, TFinal> Rest { get; init; }

    public async Task<IMaybe<TFinal>> RunAsync(TSource source)
    {
        var map = Mapper.Invoke(source);
        if (map is not Just<TMap> just)
        {
            return Maybe.Maybe.Nothing<TFinal>();
        }

        return await Rest.RunAsync(just.Value);
    }

    public ICommandPipe2<TSource, TNewFinal> WithTail<TNewFinal>(
        ICommandPipe2<TFinal, TNewFinal> tail)
    {
        return new Map2<TSource, TMap, TNewFinal>()
        {
            Mapper = Mapper,
            Rest = Rest.WithTail(tail)
        };
    }
}