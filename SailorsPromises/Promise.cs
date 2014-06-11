//The MIT License (MIT)
//
//Copyright (c) 2014 Matteo Canessa (matcanessa@gmail.com)
//
//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in
//all copies or substantial portions of the Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace SailorsPromises
{
	/// <summary>
	/// Description of Promise.
	/// </summary>
	public class Promise : IPromise
	{
		List<Action<object>> onFulfilledCallbacks = new List<Action<object>>();
		List<Action<Exception>> onRejectedCallbacks = new List<Action<Exception>>();
		List<Action<object>> onNotifyCallbacks = new List<Action<object>>();
		List<Action> onFinalllyCallbacks = new List<Action>();

		object value;
		Exception reason;
		Promise followingPromise;

		PromiseState promiseState = PromiseState.Pending;

		public virtual object Value
		{
			get
			{
				if (promiseState == PromiseState.Pending)
				{
					throw new InvalidOperationException("Cannot get Value from a not fulfilled promise");
				}
				return value;
			}
		}

		public virtual Exception Reason
		{
			get
			{
				if (promiseState == PromiseState.Pending)
				{
					throw new InvalidOperationException("Cannot get Reason from a not rejected promise");
				}
				return reason;
			}
		}

		internal SynchronizationContext SynchronizationContext
		{
			get;
			set;
		}

		internal protected Promise()
		{

		}

		public virtual IPromise Then(Action<object> onFulfilled)
		{
			if (onFulfilled != null)
			{
				onFulfilledCallbacks.Add(onFulfilled);
			}
			
			followingPromise = new Promise();
			if (this.SynchronizationContext != null)
			{
				followingPromise.SynchronizationContext = this.SynchronizationContext;
			}

			return followingPromise;
		}

		public virtual IPromise OnError(Action<Exception> onRejected)
		{
			if (onRejected != null)
			{
				onRejectedCallbacks.Add(onRejected);
			}

			followingPromise = new Promise();
			if (this.SynchronizationContext != null)
			{
				followingPromise.SynchronizationContext = this.SynchronizationContext;
			}

			return followingPromise;
		}

		public virtual IPromise Finally(Action onFinally)
		{
			if (onFinally != null)
			{
				onFinalllyCallbacks.Add(onFinally);
			}

			followingPromise = new Promise();
			if (this.SynchronizationContext != null)
			{
				followingPromise.SynchronizationContext = this.SynchronizationContext;
			}

			return followingPromise;
		}

		public virtual IPromise Notify(Action<object> onNotify)
		{
			if (onNotify != null)
			{
				onNotifyCallbacks.Add(onNotify);
			}

			followingPromise = new Promise();
			if (this.SynchronizationContext != null)
			{
				followingPromise.SynchronizationContext = this.SynchronizationContext;
			}

			return followingPromise;
		}

		[SuppressMessage("Microsoft.Design", "CA1031")]
		internal protected virtual void Fulfill(object value)
		{
			if (promiseState != PromiseState.Pending)
			{
				throw new InvalidOperationException("Cannot fulfill a not pending promise");
			}
			Action<Action<object>, object> action = (SynchronizationContext != null) ? (Action<Action<object>, object>)this.InvokeCall : this.Call;

			bool rejected = false;
			foreach (var onFulfilledCallback in onFulfilledCallbacks)
			{
				try
				{
					action(onFulfilledCallback, value);
				}
				catch (Exception exc)
				{
					Reject(exc);
					rejected = true;
					break;
				}
			}

			if (!rejected)
			{
				this.value = value;
				this.promiseState = PromiseState.Fulfilled;

				if (followingPromise != null)
				{
					followingPromise.Fulfill(value);
				}
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1031")]
		internal protected virtual void Reject(Exception reason)
		{
			if (promiseState != PromiseState.Pending)
			{
				throw new InvalidOperationException("Cannot reject a not pending promise");
			}
			Action<Action<Exception>, Exception> action = (SynchronizationContext != null) ? (Action<Action<Exception>, Exception>)this.InvokeCall : this.Call;

			foreach (var onRejectedCallback in onRejectedCallbacks)
			{
				try
				{
					action(onRejectedCallback, reason);
				}
				catch
				{

				}
			}
			this.reason = reason;
			this.promiseState = PromiseState.Rejected;

			if (followingPromise != null)
			{
				followingPromise.Reject(reason);
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1031")]
		internal virtual void Finally()
		{
			Action<Action> action = (SynchronizationContext != null) ? (Action<Action>)this.InvokeCall : this.Call;

			foreach (var onFinalllyCallback in onFinalllyCallbacks)
			{
				try
				{
					action(onFinalllyCallback);
				}
				catch
				{

				}
			}

			if (followingPromise != null)
			{
				followingPromise.Finally();
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1031")]
		internal virtual void Notify(object value)
		{
			Action<Action<object>, object> action = (SynchronizationContext != null) ? (Action<Action<object>, object>)this.InvokeCall : this.Call;

			foreach (var onNotifyCallback in onNotifyCallbacks)
			{
				try
				{
					action(onNotifyCallback, value);
				}
				catch
				{
					
				}
			}

			if (followingPromise != null)
			{
				followingPromise.Notify(value);
			}
		}

		private void InvokeCall<T>(Action<T> callback, object value)
		{
			SynchronizationContext.Send(new SendOrPostCallback(callback as Action<object>), value);
		}

		private void Call<T>(Action<T> callback, T value)
		{
			callback(value);
		}

		private void InvokeCall(Action callback)
		{
			SynchronizationContext.Send(new SendOrPostCallback((obj) => callback()), null);
		}

		private void Call(Action callback)
		{
			callback();
		}
	}
}
