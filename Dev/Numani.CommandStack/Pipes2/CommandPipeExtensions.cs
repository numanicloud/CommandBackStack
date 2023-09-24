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

    public static async Task<TFinal> RunAsRootAsync<TFinal>(
        this ICommandPipe2<TFinal> pipe)
    {
        while (true)
        {
            if (await pipe.RunAsync() is Just<TFinal> just)
            {
                return just.Value;
            }
        }
    }

    public static ICommandPipe2<TFinal> Map<TMap, TFinal>(
        this ICommandPipe2<TMap> origin,
        Func<TMap, IMaybe<TFinal>> mapper)
    {
        return origin.WithTail(new Map2<TMap, TFinal, TFinal>()
        {
            Mapper = mapper,
            Rest = new Tail2<TFinal>()
        });
    }

    public static ICommandPipe2<TFinal> Then<TValue, TFinal>(
        this ICommandPipe2<TValue> origin,
        Func<TValue, Task<IMaybe<TFinal>>> process)
    {
        return origin.WithTail(new Step2<TValue, TFinal, TFinal>()
        {
            Function = process,
            Rest = new Tail2<TFinal>()
        });
    }

    public static ICommandPipe2<TFinal> Bind<TValue, TFinal>(
        this ICommandPipe2<TValue> origin,
        Func<TValue, ICommandPipe2<TFinal>> binder)
    {
        return origin.WithTail(new BindPipe<TValue, TFinal, TFinal>()
        {
            Rest = new Tail2<TFinal>(),
            Binder = binder
        });
    }

    public static ICommandPipe2<TArg> ToCommandPipe<TArg>(this TArg initial)
    {
        return new EntryPipe<TArg, TArg>()
        {
            Initial = initial,
            Rest = new Tail2<TArg>()
        };
    }
}