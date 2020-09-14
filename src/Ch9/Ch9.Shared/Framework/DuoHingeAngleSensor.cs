#if __ANDROID__
using System;
using System.Linq;
using Android.OS;
using Uno.Devices.Sensors;
using Uno.Devices.Sensors.Helpers;

namespace Ch9.Views
{
	public class DuoHingeAngleSensor : INativeHingeAngleSensor
	{
		//private readonly HingeSensor _hingeSensor;

		private EventHandler<NativeHingeAngleReading> _readingChanged;

		public DuoHingeAngleSensor(object owner)
		{
			//if (!(ContextHelper.Current is Activity currentActivity))
			//{
			//	throw new InvalidOperationException("DuoHingeAngleSensor must be initialized on the UI Thread");
			//}

			//_hingeSensor = new HingeSensor(currentActivity);
		}

		public bool DeviceHasHinge => true;//_hingeSensor.HasHinge;

		public event EventHandler<NativeHingeAngleReading> ReadingChanged
		{
			add
			{
				var isFirstSubscriber = _readingChanged == null;
				_readingChanged += value;
				if (isFirstSubscriber)
				{
					//StartReading();
				}
			}
			remove
			{
				_readingChanged -= value;
				if (_readingChanged == null)
				{
					//StopReading();
				}
			}
		}

		private static DateTimeOffset TimestampToDateTimeOffset(long timestamp)
		{
			return DateTimeOffset.Now
				.AddMilliseconds(-SystemClock.ElapsedRealtime())
				.AddMilliseconds(timestamp / 1000000.0);
		}

		//private void StartReading() =>
		//	_hingeSensor.OnSensorChanged += OnSensorChanged;

		//private void StopReading() =>
		//	_hingeSensor.OnSensorChanged -= OnSensorChanged;

		//private void OnSensorChanged(object sender, HingeSensor.HingeSensorChangedEventArgs e) =>
		//	_readingChanged?.Invoke(this, new NativeHingeAngleReading(e.HingeAngle, TimestampToDateTimeOffset(e.SensorEvent.Timestamp)));

	}
}
#endif
