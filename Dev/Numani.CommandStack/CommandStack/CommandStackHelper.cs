using System.Collections.Generic;
using System.Linq;
using Numani.CommandStack.Maybe;
using Numani.CommandStack.Pipes;

namespace Numani.CommandStack
{
	public static class CommandStack
	{
		public static CommandStack<T, T> Entry<T>() => new(new Tail<T>());

		public static CommandStack<TArg, TFinal> Entry<TArg, TFinal>(CommandBody<TArg, TFinal> body)
			=> Entry<TArg>().Then(body);

		public static CommandStack<T, TResult> Fail<T, TResult>()
			=> Entry(async (T x) => Maybe.Maybe.Nothing<TResult>());

		public static CommandStack<Unit, IEnumerable<TResult>> ForEach<T, TResult>(
			IEnumerable<T> source, CommandProjector<T, TResult> projector)
		{
			IEnumerable<TResult> empty = Enumerable.Empty<TResult>();

			var seed = Entry<Unit>().Map(_ => empty.Just());

			return source.Aggregate(seed, (s, x) =>
			{
				var current = empty;

				IMaybe<Unit> Backup(IEnumerable<TResult> q)
				{
					current = q;
					return Unit.Id.Just();
				}

				return s.Map(Backup)
					.Then(projector(Entry<Unit>().Map(_ => x.Just())))
					.Map(q => current.Append(q).Just());
			});
		}
	}
}
