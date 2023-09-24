using System;
using Numani.CommandStack.Common;

namespace Numani.CommandStack.Maybe;

public static class Maybe
{
	public static IMaybe<T> Just<T>(this T value) => new Just<T>(value);

	public static IMaybe<T> Nothing<T>() => new Nothing<T>();

	public static IMaybe<T> FromNullable<T>(this T? x) where T : struct
		=> x.HasValue ? x.Value.Just() : new Nothing<T>();

	public static IMaybe<T> FromNullable<T>(this T? x) where T : class
		=> x is not null ? x.Just() : new Nothing<T>();
		
	public static TMapped Match<T, TMapped>(
		this IMaybe<T> monad, Func<T, TMapped> onJust, Func<TMapped> onNothing)
		=> monad switch
		{
			Just<T> just => onJust(just.Value),
			Nothing<T> => onNothing(),
			_ => throw new PatternLeakedException(typeof(IMaybe<T>))
		};
}