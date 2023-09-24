using System;
using System.Linq;
using System.Threading.Tasks;
using Numani.CommandStack.Maybe;
using Numani.CommandStack.TaskMaybe;

namespace Numani.CommandStack.Pipes
{
	internal record Step<TArg, TResult, TFinal>(
		CommandBody<TArg, TResult> Function,
		ICommandPipe<TResult, TFinal> Rest) : ICommandPipe<TArg, TFinal>
	{
		public async Task<IMaybe<TFinal>> Run(TArg arg, Logger logger)
		{
			logger.OutputLog("Step");
			while (true)
			{
				var result = await Function.Invoke(arg)
					.Bind(async x => (await Rest.Run(x, logger)).Just());

				if (result is Just<IMaybe<TFinal>> { Value: Nothing<TFinal> }) continue;

				return result.Join();
			}
		}

		public ICommandPipe<TArg, TNewFinal> Then<TNewFinal>(ICommandPipe<TFinal, TNewFinal> commandPipe)
			=> new Step<TArg, TResult, TNewFinal>(Function, Rest.Then(commandPipe));

		public override string ToString()
		{
			var arg = typeof(TArg).ParameterizedName();
			var result = typeof(TResult).ParameterizedName();
			var final = typeof(TFinal).ParameterizedName();
			return $"Step<{arg}, {result}, {final}> ({arg} -> {result})";
		}

		public string GetTreeString(int line) =>
			$"""
			 {line:00}: {ToString()}
			 {Rest.GetTreeString(line + 1)}
			 """;
	}
}