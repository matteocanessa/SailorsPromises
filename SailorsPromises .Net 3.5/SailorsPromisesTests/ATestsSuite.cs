//-----------------------------------------------------------------------
// <copyright file="ATestsSuite.cs" company="https://github.com/matteocanessa/SailorsPromises">
//     Copyright (c) 2014 Matteo Canessa (sailorspromises@gmail.com)
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
using SailorsPromises;
using Xunit;

namespace SailorsPromisesTests
{
    /// <summary>
    /// Description of ATestsSuite.
    /// </summary>
    public class ATestsSuite
    {
        [Fact]
        public void Sailor_method_should_return_a_Sailor_instance()
        {
           ISailor s = A.Sailor();

           //Sailor method should return a Sailor instance
           Assert.True(s.GetType().IsAssignableFrom(typeof(Sailor)));
        }
        
        [Fact]
        public void Sailor_method_should_return_a_new_Sailor_instance_every_call()
        {
           ISailor s = A.Sailor();
           ISailor s1 = A.Sailor();

           //Sailor method should return a Sailor instance
           Assert.False(Object.ReferenceEquals(s, s1));
        }
    }
}
