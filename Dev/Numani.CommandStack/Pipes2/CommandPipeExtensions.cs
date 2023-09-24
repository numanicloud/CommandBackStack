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

    public static ICommandPipe2<THead, TValue> Bind<THead, TJoint, TValue>(
        this ICommandPipe2<THead, TJoint> origin,
        Func<TJoint, ICommandPipe2<TJoint, TValue>> binder)
    {
        return origin.WithTail(new BindPipe<TJoint, TValue, TValue>()
        {
            Binder = binder,
            Rest = new Tail2<TValue>()
        });

        // ユーザー側が ICommandPipe2 の構造を知っている前提なのが変
        // ICommandPipe<THead, TJoint> -> ICommandPipe<TJoint, TValue>
        // といったメソッドを要求する方が自然

        // RunAsync をよぶ側は THead, TFinal を知っておく必要があるが、
        // 構築する側は実は THead を知っている必要がない？

        // Bindの使用者は戻り値の型に責任を持つが、開始時の値には責任を持たない
        // また、入力の型に期待を持っている
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
}