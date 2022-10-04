namespace Homicide.AI.Tasks
{
	public interface IActData
	{
	}

	public class ActData<T> : IActData
	{
		public T Data { get; set; }

		public ActData(T data)
		{
			Data = data;
		}
	}
}