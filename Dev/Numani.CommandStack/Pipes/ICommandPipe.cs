using System.Threading.Tasks;
using Numani.CommandStack.Maybe;

namespace Numani.CommandStack.Pipes;

public interface ICommandPipe<TFinal>
{
    Task<IMaybe<TFinal>> RunAsync();
    ICommandPipe<TNewFinal> WithTail<TNewFinal>(ICommandPipe<TFinal, TNewFinal> tail);
}

public interface ICommandPipe<in TArg, TFinal>
{
    Task<IMaybe<TFinal>> RunAsync(TArg source);
    ICommandPipe<TArg, TNewFinal> WithTail<TNewFinal>(ICommandPipe<TFinal, TNewFinal> tail);
    string ToTreeString(int indent);
}