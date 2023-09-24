using System;
using System.Threading.Tasks;
using Numani.CommandStack.Maybe;
using Numani.CommandStack.Tasks;

namespace Numani.CommandStack.Pipes;

internal class Join<TCont, TArg, TResult, TFinal> : ICommandPipe<TCont, TFinal>
{
    private readonly Func<TCont, CommandStack<TArg, TResult>> _binder;
    private readonly ICommandPipe<TResult, TFinal> _rest;

    public Join(Func<TCont, CommandStack<TArg, TResult>> binder, ICommandPipe<TResult, TFinal> rest)
    {
        _binder = binder;
        _rest = rest;
    }

    public string GetTreeString(int line)
    {
        return "Join";
    }

    public async Task<IMaybe<TFinal>> Run(TCont arg, Logger logger)
    {
        var commandStack = _binder(arg);

        throw new NotImplementedException();
    }

    public ICommandPipe<TCont, TNewFinal> Then<TNewFinal>(ICommandPipe<TFinal, TNewFinal> commandPipe)
    {
        return new Join<TCont, TArg, TResult, TNewFinal>(_binder, _rest.Then(commandPipe));
    }
}