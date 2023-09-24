using System.Threading.Tasks;
using Numani.CommandStack.Maybe;

namespace Numani.CommandStack.Pipes2;

public interface ICommandPipe2<in TArg, TFinal>
{
    Task<IMaybe<TFinal>> RunAsync(TArg source);
    ICommandPipe2<TArg, TNewFinal> WithTail<TNewFinal>(ICommandPipe2<TFinal, TNewFinal> tail);
}