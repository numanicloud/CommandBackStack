using System;
using System.Threading.Tasks;
using Numani.CommandStack.Maybe;
using Numani.CommandStack.Tasks;

namespace Numani.CommandStack.TaskMaybe
{
	public static class TaskMaybeMonad
	{
		public static Task<IMaybe<TResult>> FMap<T, TResult>(
			this Task<IMaybe<T>> m, Func<T, TResult> mapper,
			TaskScheduler? taskScheduler = null)
		{
			return m.ContinueWith(x => x.Result.FMap(mapper),
				taskScheduler ?? TaskScheduler.FromCurrentSynchronizationContext());
		}

		public static Task<IMaybe<T>> Join<T>(this Task<IMaybe<Task<IMaybe<T>>>> monad)
			=> monad.Bind(MashMaybe<T>);

		public static Task<IMaybe<TResult>> Bind<T, TResult>(
			this Task<IMaybe<T>> m, Func<T, Task<IMaybe<TResult>>> binder)
			=> m.FMap(binder).Join();

		private static Task<IMaybe<T>> MashMaybe<T>(this IMaybe<Task<IMaybe<T>>> monad)
			=> monad.Match(just => just,
				() => Maybe.Maybe.Nothing<T>().FromResult());
	}
}
