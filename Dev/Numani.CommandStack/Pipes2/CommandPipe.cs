using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Numani.CommandStack.Maybe;

namespace Numani.CommandStack.Pipes2;

public static class CommandPipe
{
    public static ICommandPipe2<T, T> Entry<T>()
    {
        return new Tail2<T>();
    }

    // TArg は常に Unit であることができる？
    public static ICommandPipe2<Unit, IEnumerable<TFinal>> ForEach2<TEntity, TFinal>(
        IEnumerable<TEntity> source,
        Func<TEntity, ICommandPipe2<Unit, TFinal>> iterator)
    {
        var result = Enumerable.Empty<TFinal>();
        var pipes = source.Select(x => iterator(x)
            .WithTail(new Map2<TFinal, Unit, Unit>()
            {
                Mapper = k =>
                {
                    result = result.Append(k);
                    return Unit.Id.Just();
                },
                Rest = new Tail2<Unit>()
            })).ToArray();

        return pipes.Aggregate(Entry<Unit>(), (seed, x) =>
        {
            return seed.Concat(_ => x);
        }).Map(_ => result.Just());
    }
}

public sealed class ForEachPipe<TArg, TEntity, TFinal> : ICommandPipe2<TArg, IEnumerable<TFinal>>
{
    public required IEnumerable<TEntity> Collection { get; init; }
    public required Func<TEntity, ICommandPipe2<TArg, TFinal>> Iterator { get; init; }
    
    public async Task<IMaybe<IEnumerable<TFinal>>> RunAsync(TArg source)
    {
        
        
        var list = new List<TFinal>();
        await foreach (var item in Stream())
        {
            if (item is Just<TFinal> just)
            {
                list.Add(just.Value);
            }
        }

        return list.Any()
            ? list.AsEnumerable().Just()
            : Maybe.Maybe.Nothing<IEnumerable<TFinal>>();

        async IAsyncEnumerable<IMaybe<TFinal>> Stream()
        {
            Stack<ICommandPipe2<TArg, TFinal>> past = new();
            Stack<ICommandPipe2<TArg, TFinal>> future = new();

            foreach (var entity in Collection.Reverse())
            {
                future.Push(Iterator(entity));
            }

            while (future.Any())
            {
                var iterator = future.Pop();
                var result = await iterator.RunAsync(source);
                if (result is Just<TFinal> just)
                {
                    past.Push(iterator);
                    yield return just;
                }
                else
                {
                    future.Push(iterator);
                    if (!past.Any())
                    {
                        yield break;
                    }

                    future.Push(past.Pop());
                }
            }
        }
    }

    public ICommandPipe2<TArg, TNewFinal> WithTail<TNewFinal>(ICommandPipe2<IEnumerable<TFinal>, TNewFinal> tail)
    {
        throw new NotImplementedException();
    }
}