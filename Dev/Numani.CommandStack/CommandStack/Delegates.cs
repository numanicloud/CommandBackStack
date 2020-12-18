using System.Threading.Tasks;
using Numani.CommandStack.Maybe;

namespace Numani.CommandStack
{
	public delegate Task<IMaybe<TResult>> CommandBody<in TArg, TResult>(TArg arg);
	public delegate IMaybe<TResult> CommandMapper<in TArg, TResult>(TArg arg);
	public delegate CommandStack<Unit, TResult> CommandProjector<T, TResult>(CommandStack<Unit, T> arg);
}
