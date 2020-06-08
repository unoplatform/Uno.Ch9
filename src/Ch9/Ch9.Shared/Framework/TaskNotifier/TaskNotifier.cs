using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Ch9
{
	public class TaskNotifier<TResult> : ITaskNotifier<TResult>
	{
		private readonly Action<Exception> _onFaulted;
		private readonly TaskScheduler _dispatcherTaskScheduler;
		private readonly TimeSpan _minimumLoadingDuration;

		public TaskNotifier(
			Task<TResult> task, 
			Action<Exception> onFaulted = null, 
			TaskScheduler dispatcherTaskScheduler = null,
			TimeSpan? minimumLoadingDuration = null
		)
		{
			_dispatcherTaskScheduler = dispatcherTaskScheduler;
			_onFaulted = onFaulted;
			_minimumLoadingDuration = minimumLoadingDuration ?? TimeSpan.FromMilliseconds(300);

			Task = task;

			if (!task.IsCompleted)
			{
				RunTask(task);
			}
			else if (task.IsFaulted && Connectivity.NetworkAccess != NetworkAccess.Internet)
			{
				IsInternetFaulted = true;
			}
		}

		/// <inheritdoc />
		public event PropertyChangedEventHandler PropertyChanged;

		/// <inheritdoc />
		public Task<TResult> Task { get; }

		/// <inheritdoc />
		public TResult Result => Task.Status == TaskStatus.RanToCompletion
					? Task.Result
					: default;

		/// <inheritdoc />
		public TaskStatus Status => Task.Status;

		/// <inheritdoc />
		public bool IsCanceled => Task.IsCanceled;

		/// <inheritdoc />
		public bool IsCompleted => Task.IsCompleted;

		/// <inheritdoc />
		public bool IsExecuting => !Task.IsCompleted;

		/// <inheritdoc />
		public bool IsFaulted => Task.IsFaulted;

		/// <inheritdoc />
		public bool IsSuccess => Task.Status == TaskStatus.RanToCompletion;

		/// <inheritdoc />
		public bool IsInternetFaulted { get; set; }

		/// <inheritdoc />
		public AggregateException Exception => Task.Exception;

		/// <inheritdoc />
		Task ITaskNotifier.Task => Task;

		/// <inheritdoc />
		object ITaskNotifier.Result => Result;

		private static TaskScheduler GetDefaultScheduler()
		{
			return SynchronizationContext.Current == null
				? TaskScheduler.Current
				: TaskScheduler.FromCurrentSynchronizationContext();
		}

		private void RunTask(Task taskToExecute)
		{
			taskToExecute.ContinueWith(
				async task =>
				{
					// Add some delaying to avoid flickers.
					await System.Threading.Tasks.Task.Delay(_minimumLoadingDuration);

					if (task.IsFaulted)
					{
						if (Connectivity.NetworkAccess != NetworkAccess.Internet)
						{
							IsInternetFaulted = true;
							PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsInternetFaulted)));
						}

						Console.Error.WriteLine(task.Exception);
						_onFaulted?.Invoke(task.Exception);
					}

#if WINDOWS_UWP
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
#else
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Result)));
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Status)));
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsCanceled)));
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsCompleted)));
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsExecuting)));
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFaulted)));
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSuccess)));
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Exception)));
#endif
				},
				scheduler: _dispatcherTaskScheduler ?? GetDefaultScheduler());
		}
	}
}
