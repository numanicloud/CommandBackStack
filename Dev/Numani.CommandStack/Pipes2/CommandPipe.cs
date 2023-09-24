using System;
using System.Collections.Generic;
using System.Linq;
using Numani.CommandStack.Common;
using Numani.CommandStack.Maybe;

namespace Numani.CommandStack.Pipes2;

public static class CommandPipe
{
    public static ICommandPipe2<Unit> Entry()
    {
        return new EntryPipe<Unit, Unit>()
        {
            Initial = Unit.Id,
            Rest = new Tail<Unit>()
        };
    }

    public static ICommandPipe2<IEnumerable<TFinal>> ForEach<TEntity, TFinal>(
        IEnumerable<TEntity> source,
        Func<TEntity, ICommandPipe2<TFinal>> iterator)
    {
        var array = source.ToArray();
        var result = new TFinal[array.Length];

        var pipes = array.Select((x, i) =>
        {
            return iterator(x)
                .WithTail(new Map2<TFinal, Unit, Unit>()
                {
                    Mapper = k =>
                    {
                        result[i] = k;
                        return Unit.Id.Just();
                    },
                    Rest = new Tail<Unit>()
                });
        }).ToArray();

        var pipe = pipes.Aggregate(Entry(), (seed, x) => seed.Bind(_ => x))
            .Map(_ => result.AsEnumerable().Just());
        
        return pipe;
    }
}