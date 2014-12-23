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
using System.Threading;
using SailorsPromises;
using Xunit;

namespace SailorsPromisesTests
{
    /// <summary>
    /// Description of SailorTestsSuite.
    /// </summary>
    public class SailorTestsSuite
    {
        [Fact]
        public void Resolve_method_should_call_promise_fulfill_exactly_once()
        {
            string val = "iyhbiyhb";
           
            PromiseMock promiseMock = new PromiseMock();

            Sailor d = new Sailor(promiseMock);
            d.Resolve(val);

            Assert.Equal(/*1*/0, promiseMock.FulfillCalls);
        }

        [Fact]
        public void Reject_method_should_call_promise_reject_exactly_once()
        {
            Exception exc = new Exception();

            PromiseMock promiseMock = new PromiseMock();
            
            Sailor d = new Sailor(promiseMock);
            d.Reject(exc);

            Assert.Equal(1, promiseMock.RejectCalls);
        }

        [Fact]
        public void Finally_method_should_call_promise_finally_exactly_once()
        {
            int call = 0;

            Sailor d = new Sailor();
            d.Promise.Finally(delegate() { call++; });
            d.Finally();

            //Finally method should call promise finally exactly once
            Assert.Equal(1, call);
        }

        [Fact]
        public void Notify_method_should_call_promise_notify_exactly_once()
        {
            string val = "iyhbiyhb";
            int call = 0;

            Sailor d = new Sailor();
            d.Promise.Notify(delegate(object obj) { call++; });
            d.Notify(val);

            //Notify method should call promise notify exactly once
            Assert.Equal(1, call);
        }
        
        [Fact]
        public void When_method_should_run_the_Action_and_all_others_methods_async_on_another_thread()
        {
            string currentThreadName = Thread.CurrentThread.Name;
            
            Sailor d = new Sailor();
            
            //When method should be executed on a thread other than the calling one
            d.When(delegate() { Assert.NotEqual(Thread.CurrentThread.Name, currentThreadName); })
                //Then method should be executed on a thread other than the calling one
                .Then(delegate(object obj) { Assert.NotEqual(Thread.CurrentThread.Name, currentThreadName); })
                //Finally method should be executed on a thread other than the calling one
                .Finally(delegate() { Assert.NotEqual(Thread.CurrentThread.Name, currentThreadName); })
                //Let the test fail if there is an exception
                .OnError(delegate(Exception exc) { Assert.False(true); });
        }
        
        [Fact]
        public void When_method_should_call_Then_Action_when_the_Action_passed_to_it_comlpetes_without_errors()
        {
            int callNr = 0;
            int callNr1 = 0;

            Sailor d = new Sailor();
            d.When(delegate() { Thread.Sleep(500); })
                .Then(delegate(object obj) { callNr++; })
                .OnError(delegate(Exception exc) { callNr1++; });
            
            Thread.Sleep(1000);
            
            //When method should call Then Action when the Action passed to it comlpetes without errors
            Assert.Equal(1, callNr);
            Assert.Equal(0, callNr1);
        }
        [Fact]
        public void When_method_should_call_Finally_action_when_the_Action_passed_to_it_comlpetes_without_errors()
        {
            int callNr = 0;
            int callNr1 = 0;

            Sailor d = new Sailor();
            d.When(delegate() { Thread.Sleep(500); })
                .Finally(delegate() { callNr++; })
                .OnError(delegate(Exception exc) { callNr1++; });
            
            Thread.Sleep(1000);

            //When method should call Finally action when the Action passed to it comlpetes without errors
            Assert.Equal(1, callNr);
            Assert.Equal(0, callNr1);
        }
        
        [Fact]
        public void When_method_should_call_Finally_action_when_the_Action_passed_to_it_comlpetes_with_errors()
        {
            int callNr = 0;
            int callNr1 = 0;
            int callNr2 = 0;

            Sailor d = new Sailor();
            d.When(delegate() {Thread.Sleep(500); throw new Exception();})
                .Finally(delegate() { callNr++; })
                .OnError(delegate(Exception exc) { callNr1++; })
                .Then(delegate(object obj) { callNr2++; });
            
            Thread.Sleep(1000);
            
            //When method should call Finally action when the Action passed to it comlpetes with errors
            Assert.Equal(1, callNr);
            Assert.Equal(1, callNr1);
            Assert.Equal(0, callNr2);
        }
        
        [Fact]
        public void Then_action_should_be_called_on_the_same_current_thread_if_when_method_is_used_with_a_synch_context()
        {
            SynchronizationContext sc = new SynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(sc);
            
            string currentThreadName = Thread.CurrentThread.Name;

            Sailor d = new Sailor();
            //When method should be executed on the same calling thread if a synchronization context is set
            d.When(delegate() { Assert.Equal(Thread.CurrentThread.Name, currentThreadName); })
                //Then method should be executed on the same calling thread if a synchronization context is set
                .Then(delegate(object obj) { Assert.Equal(Thread.CurrentThread.Name, currentThreadName); })
                //Finally method should be executed on the same calling thread if a synchronization context is set
                .Finally(delegate() { Assert.Equal(Thread.CurrentThread.Name, currentThreadName); })
                //Let the test fail if there is an exception
                .OnError(delegate(Exception exc) { Assert.False(true); });
            
            SynchronizationContext.SetSynchronizationContext(null);
        }
    }
}
