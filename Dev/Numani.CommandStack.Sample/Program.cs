using System;
using System.Linq;
using System.Threading.Tasks;
using Numani.CommandStack;
using Numani.CommandStack.Maybe;
using Numani.CommandStack.TaskMaybe;

var sum = await CommandStack.Entry((Unit u) => InputIntegerAsync())
	.Then(x => InputIntegerAsync().FMap(p => x + p))
	.Then(y => InputIntegerAsync().FMap(p => y + p))
	.RunAsync(Unit.Id);

Console.WriteLine(sum);

var sumMul = await CommandStack.ForEach(Enumerable.Range(0, 3), it =>
	{
		return it.Then(i => InputIntegerAsync())
			.Then(x => InputIntegerAsync().FMap(p => x + p));
	})
	.Map(list => list.Aggregate((a, b) => a * b).Just())
	.RunAsync(Unit.Id);

Console.WriteLine(sumMul);

async Task<IMaybe<int>> InputIntegerAsync()
{
	while (true)
	{
		Console.Write(">");
		var input = Console.ReadLine();
		if (input == "cancel")
		{
			return Maybe.Nothing<int>();
		}

		if (int.TryParse(input, out var result))
		{
			return result.Just();
		}
	}
}