using System;
using System.Threading.Tasks;
using Numani.CommandStack.Maybe;

namespace Numani.CommandStack.Pipes.Helpers;

public static class CommandPipeExtensions
{
    public static async Task<TFinal> RunAsRootAsync<TFinal>(
        this ICommandPipe<TFinal> pipe)
    {
        while (true)
        {
            if (await pipe.RunAsync() is Just<TFinal> just)
            {
                return just.Value;
            }
        }
    }

    public static ICommandPipe<TFinal> Map<TMap, TFinal>(
        this ICommandPipe<TMap> origin,
        Func<TMap, IMaybe<TFinal>> mapper)
    {
        return origin.WithTail(new MapPipe<TMap, TFinal, TFinal>()
        {
            Mapper = mapper,
            Rest = new TailPipe<TFinal>()
        });
    }

    public static ICommandPipe<TFinal> Then<TValue, TFinal>(
        this ICommandPipe<TValue> origin,
        Func<TValue, Task<IMaybe<TFinal>>> process)
    {
        return origin.WithTail(new StepPipe<TValue, TFinal, TFinal>()
        {
            Function = process,
            Rest = new TailPipe<TFinal>()
        });
    }

    public static ICommandPipe<TFinal> Bind<TValue, TFinal>(
        this ICommandPipe<TValue> origin,
        Func<TValue, ICommandPipe<TFinal>> binder,
        bool isChunk = false)
    {
        return origin.WithTail(new BindPipe<TValue, TFinal, TFinal>()
        {
            Rest = new TailPipe<TFinal>(),
            Binder = binder,
            IsChunk = isChunk
        });
    }

    public static ICommandPipe<TArg> ToCommandPipe<TArg>(this TArg initial)
    {
        return new EntryPipe<TArg, TArg>()
        {
            Initial = initial,
            Rest = new TailPipe<TArg>()
        };
    }
}