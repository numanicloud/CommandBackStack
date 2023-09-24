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

		public async Task<TFinal> RunAsync(TArg arg, bool showLog = false)
		{
			var logger = new Logger(showLog);
			while (true)
			{
				if (await _head.Run(arg, logger) is Just<TFinal> final)
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

		public override string ToString() => _head.GetTreeString(0);
	}

	internal sealed class Logger
	{
		private readonly bool _showLog;
		private int _currentLine = 0;

		public Logger(bool showLog)
		{
			_showLog = showLog;
		}
		
		public void OutputLog(string title)
		{
			if (!_showLog) return;

			Console.WriteLine($"[CommandStack] {_currentLine}: {title}");
			_currentLine += 1;
		}
	}
}