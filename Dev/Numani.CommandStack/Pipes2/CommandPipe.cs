namespace Numani.CommandStack.Pipes2;

public static class CommandPipe
{
    public static ICommandPipe2<T, T> Entry<T>()
    {
        return new Tail2<T>();
    }
}