using System;

namespace Numani.CommandStack.Common;

public class PatternLeakedException : Exception
{
	public object? ActualValue { get; }

	public PatternLeakedException(object actualValue)
	{
		ActualValue = actualValue;
	}

	public override string Message => ActualValue switch
	{
		null => $"パターンマッチングが網羅されていません。",
		var v => $"パターンマッチングが網羅されていません。 実際の値:{v}, その値の型:{v.GetType()}",
	};
}