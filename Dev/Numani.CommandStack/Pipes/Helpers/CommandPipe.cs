using System;
using System.Collections.Generic;
using System.Linq;
using Numani.CommandStack.Common;
using Numani.CommandStack.Maybe;

namespace Numani.CommandStack.Pipes.Helpers;

public static class CommandPipe
{
    public static ICommandPipe<Unit> Entry()
    {
        return new EntryPipe<Unit, Unit>()
        {
            Initial = Unit.Id,
            Rest = new TailPipe<Unit>()
        };
    }

    public static ICommandPipe<IEnumerable<TFinal>> ForEach<TEntity, TFinal>(
        IEnumerable<TEntity> source,
        Func<TEntity, ICommandPipe<TFinal>> iterator)
    {
        var array = source.ToArray();
        var result = new TFinal[array.Length];

        var pipes = array.Select((x, i) =>
        {
            return iterator(x)
                .WithTail(new MapPipe<TFinal, Unit, Unit>()
                {
                    Mapper = k =>
                    {
                        result[i] = k;
                        return Unit.Id.Just();
                    },
                    Rest = new TailPipe<Unit>()
                });
        }).ToArray();

        return pipes.Aggregate(Entry(), (seed, x) => seed.Bind(_ => x, true))
            .Map(_ => result.AsEnumerable().Just());
    }
}