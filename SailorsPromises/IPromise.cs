﻿// <copyright file="IPromise.cs" company="https://github.com/matteocanessa/SailorsPromises">
//     Copyright (c) 2014 Matteo Canessa (sailorspromises@gmail.com)
// </copyright>
// <summary>Promise interface</summary>
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

    /// <summary>
    /// Promise interface.
    /// </summary>
    public interface IPromise
    {
        /// <summary>
        /// Result of a fulfilled promise.
        /// </summary>
        object Value { get; }

                
        /// <summary>
        /// Exception for a rejected promise.
        /// </summary>
        Exception Reason { get; }

        /// <summary>
        /// Specify the action to be executed if the promise is fulfilled.
        /// </summary>
        /// <param name="onFulfilled">The action to be executed if the promise is fulfilled.</param>
        /// <returns>A new instance of a promise chained to this one.</returns>
        IPromise Then(Action<object> onFulfilled);

        /// <summary>
        /// Specify the action to b executed if the promise is rejected due to an exception.
        /// </summary>
        /// <param name="onRejected">The action to b executed if the promise is rejected due to an exception.</param>
        /// <returns>A new instance of a promise chained to this one.</returns>
        IPromise OnError(Action<Exception> onRejected);
        
        IPromise Finally(Action onFinally);
        
        IPromise Notify(Action<object> onNotify);
    }
}
