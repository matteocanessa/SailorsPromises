// <copyright file="Promise.cs" company="https://github.com/matteocanessa/SailorsPromises">
//     Copyright (c) 2015 Matteo Canessa (sailorspromises@gmail.com)
// </copyright>
// <summary>Promise implementation</summary>
//
// The MIT License (MIT)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

namespace SailorsPromises
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;

    /// <summary>
    /// Internal class implementing the <code>IPromise</code> interface
    /// </summary>
	internal class Promise : SyncHelper, IPromise
    {
        /// <remarks />
        List<Action<object>> onFulfilledCallbacks = new List<Action<object>>();
        /// <remarks />
        List<Action<Exception>> onRejectedCallbacks = new List<Action<Exception>>();
        /// <remarks />
        List<Action<object>> onNotifyCallbacks = new List<Action<object>>();
        /// <remarks />
        List<Action> onFinallyCallbacks = new List<Action>();

        /// <remarks />
        object value;
        /// <remarks />
        Exception reason;
        /// <remarks />
        Promise followingPromise;
        /// <remarks />
		PromiseState promiseState = PromiseState.Pending;

		internal PromiseState PromiseState
		{
			get { return promiseState; }
		}

        /// <remarks />
        internal Promise()
        {
        }
        
        public virtual object Value
        {
            get
            {
                if (this.promiseState != PromiseState.Fulfilled)
                {
                    throw new InvalidOperationException("Cannot get Value from a not fulfilled promise");
                }
                
                return this.value;
            }
        }

        public virtual Exception Reason
        {
            get
            {
                if (this.promiseState != PromiseState.Rejected)
                {
                    throw new InvalidOperationException("Cannot get Reason from a not rejected promise");
                }
                
                return this.reason;
            }
        }

        public virtual IPromise Then(Action<object> onFulfilled)
        {
            if (onFulfilled != null)
            {
                this.onFulfilledCallbacks.Add(onFulfilled);
            }
            
            this.followingPromise = new Promise();
            if (this.SynchronizationContext != null)
            {
                this.followingPromise.SynchronizationContext = this.SynchronizationContext;
            }

            return this.followingPromise;
        }

		public virtual IPromise OnError(Action<Exception> onRejected)
        {
            if (onRejected != null)
            {
                this.onRejectedCallbacks.Add(onRejected);
            }

            this.followingPromise = new Promise();
            if (this.SynchronizationContext != null)
            {
                this.followingPromise.SynchronizationContext = this.SynchronizationContext;
            }

            return this.followingPromise;
        }

        public virtual IPromise Finally(Action onFinally)
        {
            if (onFinally != null)
            {
                this.onFinallyCallbacks.Add(onFinally);
            }

            this.followingPromise = new Promise();
            if (this.SynchronizationContext != null)
            {
                this.followingPromise.SynchronizationContext = this.SynchronizationContext;
            }

            return this.followingPromise;
        }

        public virtual IPromise Notify(Action<object> onNotify)
        {
            if (onNotify != null)
            {
                this.onNotifyCallbacks.Add(onNotify);
            }

            this.followingPromise = new Promise();
            if (this.SynchronizationContext != null)
            {
                this.followingPromise.SynchronizationContext = this.SynchronizationContext;
            }

            return this.followingPromise;
        }

		[SuppressMessage("Microsoft.Design", "CA1031", Justification = "I need the exception to be generic to catch all types of exceptions")]
        internal virtual void Finally()
        {
            Action<Action> action = (SynchronizationContext != null) ? (Action<Action>)this.InvokeCall : this.Call;

            foreach (Action onFinalllyCallback in this.onFinallyCallbacks)
            {
                try
                {
                    action(onFinalllyCallback);
                }
                catch
                {
                }
            }

            if (this.followingPromise != null)
            {
                this.followingPromise.Finally();
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1031", Justification = "I need the exception to be generic to catch all types of exceptions")]
        internal virtual void Notify(object value)
        {
            ActionObjectDelegate action = 
                (SynchronizationContext != null) ?
                new ActionObjectDelegate(this.InvokeCall) :
                new ActionObjectDelegate(this.Call);

            foreach (Action<object> onNotifyCallback in this.onNotifyCallbacks)
            {
                try
                {
                    action(onNotifyCallback, value);
                }
                catch
                {
                }
            }

            if (this.followingPromise != null)
            {
                this.followingPromise.Notify(value);
            }
        }
        
        [SuppressMessage("Microsoft.Design", "CA1031", Justification = "I need the exception to be generic to catch all types of exceptions")]
        internal virtual void Fulfill(object value)
        {
            if (this.promiseState != PromiseState.Pending)
            {
                throw new InvalidOperationException("Cannot fulfill a not pending promise");
            }
            
            ActionObjectDelegate action = 
                (SynchronizationContext != null) ?
                new ActionObjectDelegate(this.InvokeCall) :
                new ActionObjectDelegate(this.Call);

            bool rejected = false;
            foreach (Action<object> onFulfilledCallback in this.onFulfilledCallbacks)
            {
                try
                {
                    action(onFulfilledCallback, value);
                }
                catch (Exception exc)
                {
                    this.Reject(exc);
                    rejected = true;
                    break;
                }
            }

            if (!rejected)
            {
                this.value = value;
                this.promiseState = PromiseState.Fulfilled;

                if (this.followingPromise != null)
                {
                    this.followingPromise.Fulfill(value);
                }
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1031", Justification = "I need the exception to be generic to catch all types of exceptions")]
        internal virtual void Reject(Exception reason)
        {
            if (this.promiseState != PromiseState.Pending)
            {
                throw new InvalidOperationException("Cannot reject a not pending promise");
            }
            
            ActionExceptionDelegate action = 
                (SynchronizationContext != null) ?
                new ActionExceptionDelegate(this.InvokeCall) :
                new ActionExceptionDelegate(this.Call);

            foreach (Action<Exception> onRejectedCallback in this.onRejectedCallbacks)
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

            if (this.followingPromise != null)
            {
                this.followingPromise.Reject(reason);
            }
        }
	}
}
