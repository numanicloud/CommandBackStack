namespace Numani.CommandStack.Maybe
{
	public interface IMaybe<T>
	{
	}

	public record Just<T>(T Value) : IMaybe<T>;

	public record Nothing<T> : IMaybe<T>;
}
