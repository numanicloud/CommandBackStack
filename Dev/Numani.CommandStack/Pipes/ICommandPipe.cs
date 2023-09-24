using System.Threading.Tasks;
using Numani.CommandStack.Maybe;

namespace Numani.CommandStack.Pipes
{
	internal interface ICommandPipe<in TArg, TFinal>
	{
		string GetTreeString(int line);
		Task<IMaybe<TFinal>> Run(TArg arg, Logger logger);
		ICommandPipe<TArg, TNewFinal> Then<TNewFinal>(ICommandPipe<TFinal, TNewFinal> commandPipe);

		public ICommandPipe<TArg, TNewFinal> Then<TNewFinal>(CommandBody<TFinal, TNewFinal> func)
			=> Then(SingleStep(func));

		public ICommandPipe<TArg, TNewFinal> Map<TNewFinal>(CommandMapper<TFinal, TNewFinal> mapper)
			=> Then(new Map<TFinal, TNewFinal, TNewFinal>(mapper, new Tail<TNewFinal>()));

		public static ICommandPipe<TFinal, TNewFinal> SingleStep<TNewFinal>(
			CommandBody<TFinal, TNewFinal> body)
			=> new Step<TFinal, TNewFinal, TNewFinal>(body, new Tail<TNewFinal>());
	}
}