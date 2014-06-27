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
    /// Description of PromiseTestSuite.
    /// </summary>
    public class PromiseTestsSuite
    {
        
        [Fact]
        public void Value_get_should_throw_exception_if_state_is_not_fulfilled()
        {
            //it is not allowed to get the value of a pending promise
            
            Exception ex = Assert.Throws<InvalidOperationException>(new Assert.ThrowsDelegate(Value_get_should_throw_exception_if_state_is_not_fulfilled_ThrowsDelegate));
            
            Assert.NotNull(ex);
        }
        private void Value_get_should_throw_exception_if_state_is_not_fulfilled_ThrowsDelegate()
        {
            object obj = A.Sailor().Promise.Value;
        }

        
        [Fact]
        public void Reason_get_should_throw_exception_if_state_is_not_fulfilled()
        {
            //it is not allowed to get the reason of a pending promise
            
            Exception ex = Assert.Throws<InvalidOperationException>(new Assert.ThrowsDelegate(Reason_get_should_throw_exception_if_state_is_not_fulfilled_ThrowsDelegate));
            
            Assert.NotNull(ex);
        }
        private void Reason_get_should_throw_exception_if_state_is_not_fulfilled_ThrowsDelegate()
        {
            object obj = A.Sailor().Promise.Reason;
        }
        
        [Fact]
        public void Value_get_should_return_the_correct_value_if_state_is_fulfilled()
        {
            ISailor s = A.Sailor();
            IPromise promise = s.Promise;

            s.Resolve(3);
            
            //a fulfilled promise value should return the parameter passed to the Sailor.Fulfill method
            Assert.Equal(3, (int)s.Promise.Value);
        }

        
        [Fact]
        public void Reason_get_should_return_the_reason_why_a_promise_has_been_rejected_if_state_is_rejected()
        {
            ISailor s = A.Sailor();
            IPromise promise = s.Promise;

            Exception exc = new Exception("Exception");
            s.Reject(exc);
            
            //a rejected promise reason should return the exception passed to the Sailor.Reject method
            Assert.Equal(exc, s.Promise.Reason);
        }

        
        [Fact]
        public void Value_get_should_return_null_if_state_is_rejected()
        {
            ISailor s = A.Sailor();
            IPromise promise = s.Promise;

            s.Reject(new Exception());

            //a rejected promise value should remain null
            Assert.Null(s.Promise.Value);
        }

        
        [Fact]
        public void Reason_get_should_return_null_if_state_is_fulfilled()
        {
            ISailor s = A.Sailor();
            IPromise promise = s.Promise;

            s.Resolve(50);

            //a fulfilled promise value should remain null
            Assert.Null(s.Promise.Reason);
        }

        
        [Fact]
        public void Then_method_should_return_a_new_promise()
        {
            IPromise promise = A.Sailor().Promise;

            IPromise promise1 = promise.Then(null);

            //the Then method should return a new promise instance
            Assert.NotSame(promise1, promise);
        }

        
        [Fact]
        public void If_InvokeRequiredOn_is_called_then_the_new_promise_returned_by_the_Then_method_should_have_the_same_control()
        {
            SynchronizationContext sc = new SynchronizationContext();

            Promise promise = A.Sailor().Promise as Promise;
            promise.SynchronizationContext = sc;

            IPromise promise1 = promise.Then(null);
            
            //the InvokeRequiredOn method should set that control
            Assert.Same(promise.SynchronizationContext, sc);
            
            //the Then method should return a new promise instance
            Assert.Same((promise1 as Promise).SynchronizationContext, promise.SynchronizationContext);
        }

        
        [Fact]
        public void If_InvokeRequiredOn_is_called_then_the_new_promise_returned_by_the_Catch_method_should_have_the_same_control()
        {
            SynchronizationContext sc = new SynchronizationContext();

            Promise promise = new Promise();
            promise.SynchronizationContext = sc;

            Promise promise1 = promise.OnError(null) as Promise;
            
            //the InvokeRequiredOn method should set that control
            Assert.Same(promise.SynchronizationContext, sc);
            
            //the Then method should return a new promise instance
            Assert.Same(promise1.SynchronizationContext, promise.SynchronizationContext);
        }

        
        [Fact]
        public void If_InvokeRequiredOn_is_called_then_the_new_promise_returned_by_the_Finally_method_should_have_the_same_control()
        {
            SynchronizationContext sc = new SynchronizationContext();

            Promise promise = new Promise();
            promise.SynchronizationContext = sc;

            Promise promise1 = promise.Finally(null) as Promise;

            //the InvokeRequiredOn method should set that control            
            Assert.Same(promise.SynchronizationContext, sc);
            //the Then method should return a new promise instance
            Assert.Same(promise1.SynchronizationContext, promise.SynchronizationContext);
        }

        
        [Fact]
        public void If_InvokeRequiredOn_is_called_then_the_new_promise_returned_by_the_Notify_method_should_have_the_same_control()
        {
            SynchronizationContext sc = new SynchronizationContext();

            Promise promise = new Promise();
            promise.SynchronizationContext = sc;

            Promise promise1 = promise.Notify(null) as Promise;
            
            //the InvokeRequiredOn method should set that control            
            Assert.Same(promise.SynchronizationContext, sc);
            //the Then method should return a new promise instance
            Assert.Same(promise1.SynchronizationContext, promise.SynchronizationContext);

        }

        
        [Fact]
        public void Same_promise_instance_Fulfill_action_sequence_call_should_respect_the_Then_calls_order()
        {
            Fake fake1 = new Fake();

            Fake fake2 = new Fake();

            Promise promise = new Promise();
            promise.Then(fake1.FakeAction);
            promise.Then(fake2.FakeAction);

            promise.Fulfill(new object());

            Assert.NotNull(fake1.CallDateTime);
            Assert.NotNull(fake2.CallDateTime);
            
            //the first promise fulfill method call should happen before the second promise fulfill method call
            Assert.True(fake1.CallDateTime < fake2.CallDateTime);
        }

        
        [Fact]
        public void Same_promise_instance_Reject_action_sequence_call_should_respect_the_OnError_calls_order()
        {
            Fake fake1 = new Fake();

            Fake fake2 = new Fake();

            Promise promise = new Promise();
            promise.OnError(fake1.FakeAction);
            promise.OnError(fake2.FakeAction);

            promise.Reject(new Exception());

            Assert.NotNull(fake1.CallDateTime);
            Assert.NotNull(fake2.CallDateTime);
            
            //the first promise reject method call should happen before the second promise reject method call
            Assert.True(fake1.CallDateTime < fake2.CallDateTime);
        }

        
        [Fact]
        public void Same_promise_instance_Notify_action_sequence_call_should_respect_the_Notify_calls_order()
        {
            Fake fake1 = new Fake();

            Fake fake2 = new Fake();

            Promise promise = new Promise();
            promise.Notify(fake1.FakeAction);
            promise.Notify(fake2.FakeAction);

            promise.Notify(new object());

            Assert.NotNull(fake1.CallDateTime);
            Assert.NotNull(fake2.CallDateTime);
            
            //the first promise notify method call should happen before the second promise notify method call
            Assert.True(fake1.CallDateTime < fake2.CallDateTime);
        }

        
        [Fact]
        public void Same_promise_instance_Finally_action_sequence_call_should_respect_the_Finally_calls_order()
        {  
            Fake fake1 = new Fake();

            Fake fake2 = new Fake();

            Promise promise = new Promise();
            promise.Finally(fake1.FakeAction);
            promise.Finally(fake2.FakeAction);

            promise.Finally();

            Assert.NotNull(fake1.CallDateTime);
            Assert.NotNull(fake2.CallDateTime);
            
            //the first promise finally method call should happen before the second promise finally method call
            Assert.True(fake1.CallDateTime < fake2.CallDateTime);
        }
        
        [Fact]
        public void Subsequent_promise_instances_fulfill_action_sequence_call_should_respect_the_Then_calls_order()
        {
//            DateTime? call1 = null;
//            var fake1 = FakeItEasy.A.Fake<Fake>();
//            FakeItEasy.A.CallTo(() => fake1.FakeAction(null as object))
//                .WithAnyArguments()
//                .Invokes(() => call1 = DateTime.Now);
//
//            DateTime? call2 = null;
//            var fake2 = FakeItEasy.A.Fake<Fake>();
//            FakeItEasy.A.CallTo(() => fake2.FakeAction(null as object))
//                .WithAnyArguments()
//                .Invokes(() => { Thread.Sleep(10); call2 = DateTime.Now; });
//
//            DateTime? call3 = null;
//            var fake3 = FakeItEasy.A.Fake<Fake>();
//            FakeItEasy.A.CallTo(() => fake3.FakeAction(null as object))
//                .WithAnyArguments()
//                .Invokes(() => { Thread.Sleep(10); call3 = DateTime.Now; });
//
//            var promise = new Promise();
//            promise.Then(fake1.FakeAction)
//                .Then(fake2.FakeAction)
//                .Then(fake3.FakeAction);
//
//            promise.Fulfill(null);
//            
//            Assert.NotNull(call1);
//            Assert.NotNull(call2);
//            Assert.NotNull(call3);
//
//            //the first promise fulfill method call should happen before the second promise fulfill method call
//            Assert.True(call1 < call2);
//            //the second promise fulfill method call should happen before the third promise fulfill method call
//            Assert.True(call2 < call3);

            Fake fake1 = new Fake();
            Fake fake2 = new Fake();
            Fake fake3 = new Fake();
            
            Promise promise = new Promise();
            promise.Then(fake1.FakeAction)
                .Then(fake2.FakeAction)
                .Then(fake3.FakeAction);
            
            promise.Fulfill(null);
            
            Assert.NotNull(fake1.CallDateTime);
            Assert.NotNull(fake2.CallDateTime);
            Assert.NotNull(fake3.CallDateTime);

            //the first promise fulfill method call should happen before the second promise fulfill method call
            Assert.True(fake1.CallDateTime < fake2.CallDateTime);
            //the second promise fulfill method call should happen before the third promise fulfill method call
            Assert.True(fake2.CallDateTime < fake3.CallDateTime);
        }

        
        [Fact]
        public void First_promise_rejected_should_reject_all_subsequent_promises()
        {
            bool call1 = false;
            bool call2 = false;
            bool call3 = false;

            Promise promise1 = new Promise();
            IPromise promise2 = promise1.OnError(delegate(Exception exc) {call1 = true;});
            IPromise promise3 = promise2.OnError(delegate(Exception exc) {call2 = true;});
            promise3.OnError(delegate(Exception exc) {call3 = true;});

            promise1.Reject(new Exception());
            
            Assert.True(call1);
            Assert.True(call2);
            Assert.True(call3);
        }

        
        [Fact]
        public void Subsequent_promise_instances_the_first_fulfill_but_the_second_reject_should_reject_all_promise_from_the_second_till_the_last()
        {
            object value = new object();
            Exception exc = new Exception();

            bool then1 = false;
            bool catch2 = false;
            bool catch3 = false;
            bool catch4 = false;

            Promise promise = new Promise();
            IPromise promise1 = promise.Then(delegate(object val) {then1 = true;});
            IPromise promise2 = promise1.Then(delegate(object val) { throw exc; }).OnError(delegate(Exception e) {catch2 = true;});
            IPromise promise3 = promise2.Then(delegate(object val) { }).OnError(delegate(Exception e) {catch3 = true;});
            promise3.Then(delegate(object val) { }).OnError(delegate(Exception e) {catch4 = true;});

            promise.Fulfill(value);

            Assert.True(then1);
            Assert.True(catch2);
            Assert.True(catch3);
            Assert.True(catch4);
        }

        
        [Fact]
        public void Fulfill_method_call_on_a_not_pending_promise_should_throw_InvalidOperationException()
        {
            Promise promise = new Promise();
            promise.Fulfill(new object());

            Exception ex = Assert.Throws<InvalidOperationException>(delegate() { promise.Fulfill(new object()); });
            //Cannot call fulfill method on a not pending promise
            Assert.NotNull(ex);
        }

        
        [Fact]
        public void Reject_method_call_on_a_not_pending_promise_should_throw_InvalidOperationException()
        {
            Promise promise = new Promise();
            promise.Reject(null);

            Exception ex = Assert.Throws<InvalidOperationException>(delegate() { promise.Reject(null); });
            //Cannot call reject method on a not pending promise
            Assert.NotNull(ex);
        }

        
        [Fact]
        public void Catch_throwing_exception_should_not_prevent_the_catch_call_on_subsequent_promises()
        {
            bool call = false;

            Promise promise = new Promise();
            IPromise promise1 = promise.OnError(delegate(Exception exc) { throw new Exception(); });
            promise1.OnError(delegate(Exception exc) { call = true;});

            Assert.DoesNotThrow(new Assert.ThrowsDelegate(delegate() { promise.Reject(null);}));
            //Catch throwing exception should not prevent the catch call on subsequent promises
            Assert.True(call);
        }

        
        [Fact]
        public void Finally_throwing_exception_should_not_prevent_the_finally_call_on_subsequent_promises()
        {
            bool call = false;

            Promise promise = new Promise();
            IPromise promise1 = promise.Finally(delegate() { throw new Exception(); });
            promise1.Finally(delegate() {call = true;});
            
            Assert.DoesNotThrow(new Assert.ThrowsDelegate(delegate() { promise.Finally(); }));
            //Finally throwing exception should not prevent the catch call on subsequent promises
            Assert.True(call);
        }

        
        [Fact]
        public void Notify_throwing_exception_should_not_prevent_the_notify_call_on_subsequent_promises()
        {
            bool call = false;

            Promise promise = new Promise();
            IPromise promise1 = promise.Notify(delegate(object obj) { throw new Exception(); });
            promise1.Notify(delegate(object obj) { call = true; });
            
            Assert.DoesNotThrow(new Assert.ThrowsDelegate(delegate() { promise.Notify(null as object); }));
            //Notify throwing exception should not prevent the catch call on subsequent promises
            Assert.True(call);
        }
    }

    //=========================================================================
    public class Fake
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
