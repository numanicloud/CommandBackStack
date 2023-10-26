using System;
using System.Threading.Tasks;
using Numani.CommandStack.Maybe;

namespace Numani.CommandStack.Pipes;

public sealed class BindPipe<TContext, TResult, TFinal> : ICommandPipe<TContext, TFinal>
{
    public required Func<TContext, ICommandPipe<TResult>> Binder { get; init; }
    public required ICommandPipe<TResult, TFinal> Rest { get; init; }
    public required bool IsChunk { get; init; }

    public async Task<IMaybe<TFinal>> RunAsync(TContext source)
    {
        // IsChunk時は、Binderで得られるワークフロー外からキャンセルされたときにワークフローの最初から実行する。
        // さもなくばワークフローの末尾に戻ってくる。
        if (IsChunk)
        {
        Back:
            var x = await Binder.Invoke(source).RunAsync();
            if (x is not Just<TResult> result)
            {
                return Maybe.Maybe.Nothing<TFinal>();
            }

            var y = await Rest.RunAsync(result.Value);
            if (y is not Just<TFinal> final)
            {
                goto Back;
            }

            return final;
        }
        else
        {
            return await Binder.Invoke(source)
                .WithTail(Rest)
                .RunAsync();
        }
    }

    public ICommandPipe<TContext, TNewFinal> WithTail<TNewFinal>(
        ICommandPipe<TFinal, TNewFinal> tail)
    {
        return new BindPipe<TContext, TResult, TNewFinal>()
        {
            Binder = Binder,
            Rest = Rest.WithTail(tail),
            IsChunk = IsChunk
        };
    }

    public string ToTreeString(int indent)
    {
        throw new NotImplementedException();
    }
}