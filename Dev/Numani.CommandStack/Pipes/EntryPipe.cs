using System.Threading.Tasks;
using Numani.CommandStack.Maybe;

namespace Numani.CommandStack.Pipes;

public sealed class EntryPipe<TArg, TFinal> : ICommandPipe<TFinal>
{
    public required TArg Initial { get; init; }
    public required ICommandPipe<TArg, TFinal> Rest { get; init; }
    
    public Task<IMaybe<TFinal>> RunAsync()
    {
        return Rest.RunAsync(Initial);
    }

    public ICommandPipe<TNewFinal> WithTail<TNewFinal>(ICommandPipe<TFinal, TNewFinal> tail)
    {
        return new EntryPipe<TArg, TNewFinal>()
        {
            Initial = Initial,
            Rest = Rest.WithTail(tail)
        };
    }
}