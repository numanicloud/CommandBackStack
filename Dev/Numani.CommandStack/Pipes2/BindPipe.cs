using System;
using System.Threading.Tasks;
using Numani.CommandStack.Maybe;

namespace Numani.CommandStack.Pipes2;

public sealed class BindPipe<TC, TE, TFinal> : ICommandPipe2<TC, TFinal>
{
    public required Func<TC, ICommandPipe2<TC, TE>> Binder { get; init; }
    public required ICommandPipe2<TE, TFinal> Rest { get; init; }
    
    public async Task<IMaybe<TFinal>> RunAsync(TC source)
    {
    Back:
        var main = await Binder.Invoke(source).RunAsync(source);
        if (main is not Just<TE> e)
        {
            return Maybe.Maybe.Nothing<TFinal>();
        }

        var rest = await Rest.RunAsync(e.Value);
        if (rest is not Just<TFinal> final)
        {
            goto Back;
        }

        return final;
    }

    public ICommandPipe2<TC, TNewFinal> WithTail<TNewFinal>(ICommandPipe2<TFinal, TNewFinal> tail)
    {
        return new BindPipe<TC, TE, TNewFinal>()
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