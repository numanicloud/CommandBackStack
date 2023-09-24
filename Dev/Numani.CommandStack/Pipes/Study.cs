using System;
using System.Threading.Tasks;
using Numani.CommandStack.Maybe;

namespace Numani.CommandStack.Pipes;

public static class StudyExtensions
{
    public static Study.ICommandPipe<T, T> Entry<T>()
    {
        return new Study.Tail<T>();
    }

    public static Study.ICommandPipe<T, TFinal> Then<T, TResult, TFinal>(
        this Study.ICommandPipe<T, TResult> origin,
        Func<TResult, Task<IMaybe<TFinal>>> function)
    {
        // この時点では "パイプの出力" と "ストリームの出力" が同じ "TFinal" 型である。
        // 別のパイプを末尾に繋ぐとこのパイプの型は変わることになる
        return origin.WithTail(new Study.Step<TResult, TFinal, TFinal>()
        {
            Function = function,
            Rest = new Study.Tail<TFinal>()
        });
    }

    public static async Task<TResult> RunAsRootAsync<T, TResult>(this Study.ICommandPipe<T, TResult> pipe, T argument)
    {
        while (true)
        {
            var result = await pipe.RunAsync(argument);
            if (result is Just<TResult> just) return just.Value;
        }
    }
}

public class Study
{
    public interface ICommandPipe<in TArg, TFinal>
    {
        Task<IMaybe<TFinal>> RunAsync(TArg source);
        ICommandPipe<TArg, TNewFinal> WithTail<TNewFinal>(ICommandPipe<TFinal, TNewFinal> tail);
    }

    public class Map<TSource, TMap, TFinal> : ICommandPipe<TSource, TFinal>
    {
        public required Func<TSource, IMaybe<TMap>> Mapper { get; init; }
        public required ICommandPipe<TMap, TFinal> Rest { get; init; }

        public async Task<IMaybe<TFinal>> RunAsync(TSource source)
        {
            var map = Mapper.Invoke(source);
            if (map is not Just<TMap> just)
            {
                return Maybe.Maybe.Nothing<TFinal>();
            }

            return await Rest.RunAsync(just.Value);
        }

        public ICommandPipe<TSource, TNewFinal> WithTail<TNewFinal>(
            ICommandPipe<TFinal, TNewFinal> tail)
        {
            return new Map<TSource, TMap, TNewFinal>()
            {
                Mapper = Mapper,
                Rest = Rest.WithTail(tail)
            };
        }
    }

    public class Tail<TFinal> : ICommandPipe<TFinal, TFinal>
    {
        public async Task<IMaybe<TFinal>> RunAsync(TFinal source)
        {
            return source.Just();
        }

        public ICommandPipe<TFinal, TNewFinal> WithTail<TNewFinal>(
            ICommandPipe<TFinal, TNewFinal> tail)
        {
            return tail;
        }
    }

    public class Step<TSource, TMap, TFinal> : ICommandPipe<TSource, TFinal>
    {
        public required Func<TSource, Task<IMaybe<TMap>>> Function { get; init; }
        public required ICommandPipe<TMap, TFinal> Rest { get; init; }

        public async Task<IMaybe<TFinal>> RunAsync(TSource source)
        {
            while (true)
            {
                var step = await Function.Invoke(source);
                if (step is not Just<TMap> just1)
                {
                    return Maybe.Maybe.Nothing<TFinal>();
                }

                var bind = await Rest.RunAsync(just1.Value);
                if (bind is not Just<TFinal> just2)
                {
                    continue;
                }

                return just2;
            }
        }

        public ICommandPipe<TSource, TNewFinal> WithTail<TNewFinal>(
            ICommandPipe<TFinal, TNewFinal> tail)
        {
            return new Step<TSource, TMap, TNewFinal>()
            {
                Function = Function,
                Rest = Rest.WithTail(tail)
            };
        }
    }
    
    public class Bulk<TSource, TMap, TFinal> : ICommandPipe<TSource, TFinal>
    {
        public required Func<TSource, ICommandPipe<TSource, TMap>> Generator { get; init; }
        public required ICommandPipe<TMap, TFinal> Rest { get; init; }
        
        public async Task<IMaybe<TFinal>> RunAsync(TSource source)
        {
            return await Generator.Invoke(source).WithTail(Rest).RunAsync(source);
        }

        public ICommandPipe<TSource, TNewFinal> WithTail<TNewFinal>(ICommandPipe<TFinal, TNewFinal> tail)
        {
            return new Bulk<TSource, TMap, TNewFinal>()
            {
                Generator = Generator,
                Rest = Rest.WithTail(tail)
            };
        }
    }
}