using System;
using System.Threading.Tasks;
using Numani.CommandStack.Maybe;

namespace Numani.CommandStack.Pipes;

public sealed class BindPipe<TContext, TResult, TFinal> : ICommandPipe2<TContext, TFinal>
{
    public required Func<TContext, ICommandPipe2<TResult>> Binder { get; init; }
    public required ICommandPipe2<TResult, TFinal> Rest { get; init; }
    
    public async Task<IMaybe<TFinal>> RunAsync(TContext source)
    {
        return await Binder.Invoke(source)
            .WithTail(Rest)
            .RunAsync();
    }

    public ICommandPipe2<TContext, TNewFinal> WithTail<TNewFinal>(ICommandPipe2<TFinal, TNewFinal> tail)
    {
        return new BindPipe<TContext, TResult, TNewFinal>()
        {
            Binder = Binder,
            Rest = Rest.WithTail(tail)
        };
    }

    public string ToTreeString(int indent)
    {
        throw new NotImplementedException();
    }
}