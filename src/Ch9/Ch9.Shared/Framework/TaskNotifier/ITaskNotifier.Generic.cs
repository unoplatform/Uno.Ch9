using System.Threading.Tasks;

namespace Ch9
{
	public interface ITaskNotifier<T> : ITaskNotifier
	{
		/// <summary>
		/// Typed task.
		/// </summary>
		new Task<T> Task { get; }

		/// <summary>
		/// Typed result.
		/// </summary>
		new T Result { get; }
	}
}
