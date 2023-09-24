using System.Threading.Tasks;
using Numani.CommandStack.Maybe;

namespace Numani.CommandStack.Pipes2;

public interface ICommandPipe2<TFinal>
{
    Task<IMaybe<TFinal>> RunAsync();
    ICommandPipe2<TNewFinal> WithTail<TNewFinal>(ICommandPipe2<TFinal, TNewFinal> tail);
}

public interface ICommandPipe2<in TArg, TFinal>
{
    Task<IMaybe<TFinal>> RunAsync(TArg source);
    ICommandPipe2<TArg, TNewFinal> WithTail<TNewFinal>(ICommandPipe2<TFinal, TNewFinal> tail);
    string ToTreeString(int indent);
}

public sealed class EntryPipe<TArg, TFinal> : ICommandPipe2<TFinal>
{
    public required TArg Initial { get; init; }
    public required ICommandPipe2<TArg, TFinal> Rest { get; init; }
    
    public Task<IMaybe<TFinal>> RunAsync()
    {
        return Rest.RunAsync(Initial);
    }

    public ICommandPipe2<TNewFinal> WithTail<TNewFinal>(ICommandPipe2<TFinal, TNewFinal> tail)
    {
        return new EntryPipe<TArg, TNewFinal>()
        {
            Initial = Initial,
            Rest = Rest.WithTail(tail)
        };
    }
}