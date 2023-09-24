using System.Threading.Tasks;
using Numani.CommandStack.Maybe;

namespace Numani.CommandStack.Pipes;

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