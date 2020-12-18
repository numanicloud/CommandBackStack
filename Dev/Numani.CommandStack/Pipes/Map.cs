using System.Threading.Tasks;
using Numani.CommandStack.Maybe;

namespace Numani.CommandStack.Pipes
{
	internal record Map<TArg, TResult, TFinal>(
		CommandMapper<TArg, TResult> Mapper,
		ICommandPipe<TResult, TFinal> Rest) : ICommandPipe<TArg, TFinal>
	{
		public async Task<IMaybe<TFinal>> Run(TArg arg)
		{
			var mapped = Mapper.Invoke(arg);
			if (mapped is not Just<TResult> just)
			{
				return new Nothing<TFinal>();
			}

			return await Rest.Run(just.Value);
		}

		public ICommandPipe<TArg, TNewFinal> Then<TNewFinal>(ICommandPipe<TFinal, TNewFinal> commandPipe)
			=> new Map<TArg, TResult, TNewFinal>(Mapper, Rest.Then(commandPipe));

		public override string ToString()
		{
			var arg = typeof(TArg).Name;
			var result = typeof(TResult).Name;
			var final = typeof(TFinal).Name;
			return $"Map<{arg}, {result}, {final}> ({arg} -> {result})";
		}
		
		public string GetTreeString() => ToString() + "\n" + Rest.GetTreeString();
	}
}