using System;

namespace Numani.CommandStack.Maybe
{
	public static class MaybeMonad
	{
		public static IMaybe<TResult> Bind<T, TResult>(this IMaybe<T> m, Func<T, IMaybe<TResult>> binder)
			=> m.FMap(binder).Join();

		public static IMaybe<T> Join<T>(this IMaybe<IMaybe<T>> m)
			=> m.Match(x => x, Maybe.Nothing<T>);

		public static IMaybe<TResult> FMap<T, TResult>(this IMaybe<T> m, Func<T, TResult> mapper)
			=> m.Match(just => mapper(just).Just(), Maybe.Nothing<TResult>);
	}
}