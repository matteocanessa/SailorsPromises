using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SailorsPromises
{
	internal class SyncHelper
	{
		/// <remarks />
		SynchronizationContext synchronizationContext;

		internal virtual SynchronizationContext SynchronizationContext
		{
			get { return this.synchronizationContext; }
			set { this.synchronizationContext = value; }
		}

		internal delegate void ActionObjectDelegate(Action<object> callback, object value);
		internal void InvokeCall(Action<object> callback, object value)
		{
			SynchronizationContext.Send(new SendOrPostCallback(callback), value);
		}

		internal delegate void ActionExceptionDelegate(Action<Exception> callback, Exception reason);
		internal void InvokeCall(Action<Exception> callback, Exception reason)
		{
			SendOrPostBag<Exception> bag = new SendOrPostBag<Exception>(callback, reason);
			SynchronizationContext.Send(new SendOrPostCallback(bag.Execute), null);
		}

		internal void Call<T>(Action<T> callback, T value)
		{
			callback(value);
		}

		internal void InvokeCall(Action callback)
		{
			SendOrPostBag bag = new SendOrPostBag(callback);
			SynchronizationContext.Send(new SendOrPostCallback(bag.Execute), null);
		}

		internal void Call(Action callback)
		{
			callback();
		}
	}
}
