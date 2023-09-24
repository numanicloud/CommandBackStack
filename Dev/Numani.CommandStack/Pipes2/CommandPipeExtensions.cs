using System;
using System.Linq;
using System.Threading.Tasks;
using Numani.CommandStack.Maybe;

namespace Numani.CommandStack.Pipes2;

public static class CommandPipeExtensions
{
    public static string Indent(this string lines, int level)
    {
        var map = lines.Split(Environment.NewLine)
            .Select(x => string.Join("", Enumerable.Repeat("\t", level)) + x);
        return string.Join(Environment.NewLine, map);
    }
    
    public static async Task<TFinal> RunAsRootAsync<TSource, TFinal>(
        this ICommandPipe2<TSource, TFinal> pipe,
        TSource source)
    {
        while (true)
        {
            if (await pipe.RunAsync(source) is Just<TFinal> just)
            {
                return just.Value;
            }
        }
    }

    public static ICommandPipe2<TSource, TFinal> Then<TSource, TMap, TFinal>(
        this ICommandPipe2<TSource, TMap> origin,
        Func<TMap, Task<IMaybe<TFinal>>> process)
    {
        return origin.WithTail(new Step2<TMap, TFinal, TFinal>()
        {
            Function = process,
            Rest = new Tail2<TFinal>()
        });
    }

    public static ICommandPipe2<TSource, TFinal> Map<TSource, TMap, TFinal>(
        this ICommandPipe2<TSource, TMap> origin,
        Func<TMap, IMaybe<TFinal>> mapper)
    {
        return origin.WithTail(new Map2<TMap, TFinal, TFinal>()
        {
            Mapper = mapper,
            Rest = new Tail2<TFinal>()
        });
    }

    public static ICommandPipe2<TSource, TFinal> Concat<TSource, TMap, TFinal>(
        this ICommandPipe2<TSource, TMap> origin,
        Func<TMap, ICommandPipe2<TMap, TFinal>> generator)
    {
        return origin.WithTail(new DynamicPipe<TMap, TFinal, TFinal>()
        {
            Generator = generator,
            Rest = new Tail2<TFinal>()
        });
    }

    public static ICommandPipe2<TSource, TFinal> Concat<TSource, TMap, TFinal>(
        this ICommandPipe2<TSource, TMap> origin,
        ICommandPipe2<TMap, TFinal> second)
    {
        return origin.WithTail(new PipeGroup<TMap, TFinal, TFinal>()
        {
            Embedded = second,
            Rest = new Tail2<TFinal>()
        });
    }
}