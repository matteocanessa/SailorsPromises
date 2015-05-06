//-----------------------------------------------------------------------
// <copyright file="Fake.cs" company="https://github.com/matteocanessa/SailorsPromises">
//     Copyright (c) 2015 Matteo Canessa (sailorspromises@gmail.com)
// </copyright>
// <summary></summary>
//-----------------------------------------------------------------------
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

using System;

namespace SailorsPromisesTests
{
    /// <summary>
    /// Fake helper class.
    /// </summary>
    internal class Fake
    {
        DateTime? callDateTime;
        
        public DateTime? CallDateTime {
            get { return callDateTime; }
        }
        
        public virtual void FakeAction(object obj)
        {
            System.Threading.Thread.Sleep(500);
            this.callDateTime = DateTime.Now;
        }

        public virtual void FakeAction()
        {
            System.Threading.Thread.Sleep(500);
            this.callDateTime = DateTime.Now;
        }

        public virtual void FakeAction(Exception exc)
        {
            System.Threading.Thread.Sleep(500);
            this.callDateTime = DateTime.Now;
        }
    }
}
