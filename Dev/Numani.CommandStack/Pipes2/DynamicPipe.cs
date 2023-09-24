using System;
using System.Threading.Tasks;
using Numani.CommandStack.Maybe;

namespace Numani.CommandStack.Pipes2;

public sealed class DynamicPipe<TSource, TMap, TFinal> : ICommandPipe2<TSource, TFinal>
{
    public required Func<TSource, ICommandPipe2<TSource, TMap>> Generator { get; init; }
    public required ICommandPipe2<TMap, TFinal> Rest { get; init; }
    
    public async Task<IMaybe<TFinal>> RunAsync(TSource source)
    {
        return await Generator.Invoke(source).WithTail(Rest).RunAsync(source);
    }

    public ICommandPipe2<TSource, TNewFinal> WithTail<TNewFinal>(ICommandPipe2<TFinal, TNewFinal> tail)
    {
        return new DynamicPipe<TSource, TMap, TNewFinal>()
        {
            Generator = Generator,
            Rest = Rest.WithTail(tail)
        };
    }
}