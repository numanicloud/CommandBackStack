using System;

namespace Numani.CommandStack
{
	public class PatternLeakedException : Exception
	{
		public object? ActualValue { get; }

		public PatternLeakedException()
		{
			ActualValue = null;
		}

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
}
