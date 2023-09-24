using System;
using Numani.CommandStack.Maybe;
using Numani.CommandStack.Pipes;
using Numani.CommandStack.Tasks;

namespace Numani.CommandStack
{
    public static class CommandStackMonad
    {
        public static CommandStack<TArg, TNewFinal> FMap<TArg, TFinal, TNewFinal>(
            this CommandStack<TArg, TFinal> monad, Func<TFinal, TNewFinal> mapper)
            => monad.Map(x => mapper(x).Just());

        public static CommandStack<TArg, TFinal> Join<TArg, TFinal>(
            this CommandStack<TArg, CommandStack<TArg, TFinal>> monad)
        {
            // monad :: stack(arg, stack(arg, final))

            // Bind(stack(arg, stack(arg, final))) -> stack(arg, final) の分解
            // すべての arg は同じ値であるはずなので、一番左のargの正体を突き止めればいい
            // stack(arg, a') -> a' が見つかれば早いが……
            // 最初の arg を捨てたstackに直すという手がありそう

            // stack(arg, a') * arg -> a' なのだが arg に何を使うべきか
            // そもそも RunAsync をここで呼ぶべきでは無いとも思う

            // ここが呼ばれるタイミングはモナドを構築する末尾のはずだから、
            // それを前提に考えれば簡単かもしれない

            // CommandStack型が最初しか使わない変数の型 TArgs と組になっているのが良くない？
            // 継続渡しの考え方を参考に整理できないだろうか
            
            // 根本のスタックから分岐する形のスタックを形成するためのメソッドであることを意識すると考えやすい？
            // Thenしたときの挙動、RunAsyncしたときの挙動、キャンセル時の挙動に注目したい
            
            

            // ここでRunAsyncしているパイプが親パイプと繋がっていない問題がある
            TArg arg = default;
            return CommandStack.Entry<TArg>()
                .Do(x => arg = x)
                .Then(monad)
                .Then(async phase => await phase.RunAsync(arg, true).FMap(x => x.Just()));
        }

        public static CommandStack<TArg, TNewFinal> Bind2<TArg, TFinal, TNewFinal>(
            this CommandStack<TArg, TFinal> monad,
            Func<TFinal, CommandStack<TArg, TNewFinal>> binder)
        {
            // RunAsync の TArg はモナド全体が実行されるときに判明する値なので、ここでRunAsyncは呼ばないはず
            // source: stack(arg, final)
            // fmap: stack(arg, final) -> (final -> stack(arg, newFinal)) -> stack(arg, stack(arg, newFinal))
            // join: stack(arg, stack(arg, newFinal)) -> stack(arg, newFinal)

            return monad.FMap(binder).Join();
        }

        public static CommandStack<TArg, TNewFinal> Bind<TArg, TFinal, TNewFinal>(
            this CommandStack<TArg, TFinal> monad,
            Func<TFinal, CommandStack<TArg, TNewFinal>> binder)
        {
            // どのような CommandStack が生成されるのかは動的に決まることに注意が必要
            // つまり、ここで CommandStack を接続するとき末尾でない可能性がある

            return monad.FMap(binder).Join();
        }
    }
}