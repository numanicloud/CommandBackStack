using System.Threading.Tasks;
using Numani.CommandStack.Maybe;

namespace Numani.CommandStack.Pipes
{
	internal class Tail<TFinal> : ICommandPipe<TFinal, TFinal>
	{
		public async Task<IMaybe<TFinal>> Run(TFinal arg) => arg.Just();

		public ICommandPipe<TFinal, TNewFinal> Then<TNewFinal>(ICommandPipe<TFinal, TNewFinal> commandPipe) => commandPipe;
		
		public override string ToString() => $"Tail<{typeof(TFinal).Name}>";

		public string GetTreeString() => ToString();
	}
}