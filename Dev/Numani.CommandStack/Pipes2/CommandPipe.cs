using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Numani.CommandStack.Maybe;

namespace Numani.CommandStack.Pipes2;

public static class CommandPipe
{
    public static ICommandPipe2<T, T> Entry<T>()
    {
        return new Tail2<T>();
    }

    public static ICommandPipe2<Unit, IEnumerable<TFinal>> ForEach2<TEntity, TFinal>(
        IEnumerable<TEntity> source,
        Func<TEntity, ICommandPipe2<Unit, TFinal>> iterator)
    {
        var array = source.ToArray();
        var result = new TFinal[array.Length];
        
        var pipes = array.Select((x, i) => iterator(x)
            .WithTail(new Map2<TFinal, Unit, Unit>()
            {
                Mapper = k =>
                {
                    result[i] = k;
                    return Unit.Id.Just();
                },
                Rest = new Tail2<Unit>()
            })).ToArray();

        return pipes.Aggregate(Entry<Unit>(), (seed, x) => seed.Concat(x))
            .Map(_ => result.AsEnumerable().Just());
    }
}