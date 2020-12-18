using System;
using System.Threading.Tasks;
using Numani.CommandStack.Maybe;
using Numani.CommandStack.Pipes;

namespace Numani.CommandStack
{
	public class CommandStack<TArg, TFinal>
	{
		private readonly ICommandPipe<TArg, TFinal> _head;

		internal CommandStack(ICommandPipe<TArg, TFinal> head)
		{
			_head = head;
		}

		public async Task<TFinal> RunAsync(TArg arg)
		{
			while (true)
			{
				if (await _head.Run(arg) is Just<TFinal> final)
				{
					return final.Value;
				}
			}
		}

		public CommandStack<TArg, TNewFinal> Then<TNewFinal>(CommandStack<TFinal, TNewFinal> process)
			=> new(_head.Then(process._head));

		public CommandStack<TArg, TNewFinal> Then<TNewFinal>(CommandBody<TFinal, TNewFinal> func)
			=> new(_head.Then(func));

		public CommandStack<TArg, TNewFinal> Map<TNewFinal>(CommandMapper<TFinal, TNewFinal> mapper)
			=> new(_head.Map(mapper));

		public CommandStack<TArg, TFinal> Do(Action<TFinal> function)
			=> new(_head.Map(p =>
			{
				function.Invoke(p);
				return p.Just();
			}));

		public override string ToString() => _head.GetTreeString();
	}
}