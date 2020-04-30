using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Ch9
{
	public interface ITaskNotifier : INotifyPropertyChanged
	{
		/// <summary>
		/// The task.
		/// </summary>
		Task Task { get; }

		/// <summary>
		/// Result of the task.
		/// </summary>
		object Result { get; }

		/// <summary>
		/// Status of the task.
		/// </summary>
		TaskStatus Status { get; }

		/// <summary>
		/// Gets whether or not the task was canceled.
		/// </summary>
		bool IsCanceled { get; }

		/// <summary>
		/// Gets whether or not the task has completed.
		/// </summary>
		bool IsCompleted { get; }

		/// <summary>
		/// Gets whether or not the task is executing.
		/// </summary>
		bool IsExecuting { get; }

		/// <summary>
		/// Gets whether or not the task completed with an error.
		/// </summary>
		bool IsFaulted { get; }

		/// <summary>
		/// Gets whether or not the task completed with a success.
		/// </summary>
		bool IsSuccess { get; }

		/// <summary>
		/// Exception reported by the task; if any. 
		/// </summary>
		AggregateException Exception { get; }
	}
}
