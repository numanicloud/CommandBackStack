using System;
using Numani.CommandStack.Maybe;
using Numani.CommandStack.Tasks;

namespace Numani.CommandStack
{
	public static class CommandStackMonad
	{
		public static CommandStack<TArg, TNewFinal> FMap<TArg, TFinal, TNewFinal>(
			this CommandStack<TArg, TFinal> monad, Func<TFinal, TNewFinal> mapper)
			=> monad.Map(x => mapper(x).Just());

		public static CommandStack<TArg, TFinal> Join<TArg, TFinal>(
			this CommandStack<TArg, CommandStack<TArg, TFinal>> monad)
		{
			TArg arg = default;
			return CommandStack.Entry<TArg>().Do(x => arg = x).Then(monad)
				.Then(async phase => await phase.RunAsync(arg).FMap(x => Maybe.Maybe.Just<TFinal>(x)));
		}

		public static CommandStack<TArg, TNewFinal> Bind<TArg, TFinal, TNewFinal>(
			this CommandStack<TArg, TFinal> monad, Func<TFinal, CommandStack<TArg, TNewFinal>> binder)
			=> monad.FMap(binder).Join();
	}
}