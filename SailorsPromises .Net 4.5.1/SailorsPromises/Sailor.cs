// <copyright file="Sailor.cs" company="https://github.com/matteocanessa/SailorsPromises">
//     Copyright (c) 2014 Matteo Canessa (sailorspromises@gmail.com)
// </copyright>
// <summary>Deferred object implementation</summary>
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
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;

    /// <summary>
    /// A Sailor famous for keepeing his promises.
    /// </summary>
    internal class Sailor : ISailor
    {
        private Promise promise;

        /// <summary>
        /// Create a new instance of a Sailor
        /// </summary>
        public Sailor() : this(new Promise())
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
        
        /// <summary>
        /// Gets the <code>IPromise</code> object to manage the asynchronous operation.
        /// </summary>
        public IPromise Promise
        {
            get { return this.promise; }
        }

        /// <summary>
        /// Resolves the given promise causing the Then promise action to be called.
        /// </summary>
        /// <param name="value">The result of the deferred operation if any, null otherwise.</param>
        public void Resolve(object value)
        {
            this.promise.Fulfill(value);
        }

        /// <summary>
        /// Rejects the give promise causing the OnError action to be called.
        /// </summary>
        /// <param name="exception">The exception causing the promise to be rejected.</param>
        public void Reject(Exception exception)
        {
            this.promise.Reject(exception);
        }

        /// <summary>
        /// Calls the Finally promise action both when the promise is resolved and when it is rejected.
        /// </summary>
        /// <remarks>It works exactly like the <code>finally</code> C# keyword.</remarks>
        public void Finally()
        {
            this.promise.Finally();
        }

        /// <summary>
        /// Calls the Notify promise action to update the state of the current asynchronous operation.
        /// </summary>
        /// <param name="value">A value indicating the progress if any, otherwise null.</param>
        public void Notify(object value)
        {
            this.promise.Notify(value);
        }

        /// <summary>
        /// Executes the action asyncronously on another thread and the executes the standard promise pattern (then action if all is good, the OnError action if there are exceptions and so on).
        /// </summary>
        /// <param name="action">The action to be executed asyncronously on another thread.</param>
        /// <returns>The promise to interact with.</returns>
        [SuppressMessage("Microsoft.Design", "CA1031", Justification = "I need the exception to be generic to catch all types of exceptions")]
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
                });

            return this.Promise;
        }
    }
}
