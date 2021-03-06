﻿// <copyright file="PromiseState.cs" company="https://github.com/matteocanessa/SailorsPromises">
//     Copyright (c) 2015 Matteo Canessa (sailorspromises@gmail.com)
// </copyright>
// <summary>Promise state</summary>
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
    /// Describes the state of a give promise.
    /// </summary>
    internal enum PromiseState
    {
        /// <summary>
        /// The initial state.
        /// </summary>
        None,
        
        /// <summary>
        /// A promise to be resolved or rejected.
        /// </summary>
        Pending,
        
        /// <summary>
        /// A resolved promise.
        /// </summary>
        Fulfilled,
        
        /// <summary>
        /// A rejected promise.
        /// </summary>
        Rejected,

		/// <summary>
		/// A aborted promise.
		/// </summary>
		Aborted
    }
}
