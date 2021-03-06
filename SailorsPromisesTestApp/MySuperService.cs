﻿//-----------------------------------------------------------------------
// <copyright file="MySuperService.cs" company="https://github.com/matteocanessa/SailorsPromises">
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
using System.Threading;
using SailorsPromises;

namespace SailorsPromisesTestApp
{
    /// <summary>
    /// Description of MySuperService.
    /// </summary>
    public class MySuperService
    {
        public MySuperService()
        {
        }
        
        public IPromise Run()
        {
            ISailor sailor = A.Sailor();
            
            new Thread(
            delegate()
            {
                    try {
                        while (true) {
							//My long execution...    
							Thread.Sleep(1000);
							sailor.Notify("Something happened");
                        }
                    } catch (Exception exception) {
                        
                        sailor.Reject(exception);
                    } finally {
                        sailor.Finally();
                    }
            }
            ).Start();
            
            return sailor.Promise;
        }
    }
}
