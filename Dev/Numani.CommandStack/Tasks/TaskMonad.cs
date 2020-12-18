using System;
using System.Threading.Tasks;

namespace Numani.CommandStack.Tasks
{
	public static class TaskMonad
	{
		public static Task<T> FromResult<T>(this T value) => System.Threading.Tasks.Task.FromResult(value);

		public static Task<TResult> Bind<T, TResult>(this Task<T> m, Func<T, Task<TResult>> binder)
			=> m.FMap(binder).Join();

		public static Task<T> Join<T>(this Task<Task<T>> m) => m.Unwrap();

		public static Task<TResult> FMap<T, TResult>(this Task<T> m, Func<T, TResult> mapper)
			=> m.ContinueWith(x => mapper(x.Result));
	}
}
