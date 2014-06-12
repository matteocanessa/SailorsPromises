//The MIT License (MIT)
//
//Copyright (c) 2014 Matteo Canessa (sailorspromises@gmail.com)
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
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace SailorsPromises
{
	/// <summary>
	/// Description of Sailor.
	/// </summary>
	public class Sailor : ISailor
	{
		Promise promise;
		public IPromise Promise { get { return promise; } }

		public Sailor() : this (new Promise())
		{
		}

		internal Sailor(Promise promise)
		{
			this.promise = promise;
			SynchronizationContext synchronizationContext = SynchronizationContext.Current;

			if (synchronizationContext != null)
			{
				promise.SynchronizationContext = synchronizationContext;
			}
		}

		public void Resolve(object value)
		{
			this.promise.Fulfill(value);
		}

		public void Reject(Exception exception)
		{
			this.promise.Reject(exception);
		}

		public void Finally()
		{
			this.promise.Finally();
		}

		public void Notify(object value)
		{
			this.promise.Notify(value);
		}

		[SuppressMessage("Microsoft.Design", "CA1031")]
		public IPromise When(Action action)
		{
			ThreadPool.QueueUserWorkItem(
				(obj)
				=>
				{
					try
					{
						action();
						Resolve(null);
					}
					catch (Exception exc)
					{
						Reject(exc);
					}
					finally
					{
						Finally();
					}
				}
				);

			return Promise;
		}
	}
}
