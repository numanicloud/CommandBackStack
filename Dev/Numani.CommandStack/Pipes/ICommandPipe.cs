using System.Threading.Tasks;
using Numani.CommandStack.Maybe;

namespace Numani.CommandStack.Pipes;

public interface ICommandPipe2<TFinal>
{
    Task<IMaybe<TFinal>> RunAsync();
    ICommandPipe2<TNewFinal> WithTail<TNewFinal>(ICommandPipe<TFinal, TNewFinal> tail);
}

public interface ICommandPipe<in TArg, TFinal>
{
    Task<IMaybe<TFinal>> RunAsync(TArg source);
    ICommandPipe<TArg, TNewFinal> WithTail<TNewFinal>(ICommandPipe<TFinal, TNewFinal> tail);
    string ToTreeString(int indent);
}