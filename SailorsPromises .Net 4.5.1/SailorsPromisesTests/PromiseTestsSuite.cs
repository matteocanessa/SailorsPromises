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
using FakeItEasy;
using FluentAssertions;
using SailorsPromises;
using Xunit;

namespace SailorsPromisesTests
{
    /// <summary>
    /// Description of PromiseTestSuite.
    /// </summary>
    public class PromiseTestsSuite
    {
        //---------------------------------------------------------------------
        [Fact]
        public void Value_get_should_throw_exception_if_state_is_not_fulfilled()
        {
            Action action = () => { var val = new Promise().Value; };

            action.ShouldThrow<InvalidOperationException>("it is not allowed to get a value of a pending promise");
        }

        //---------------------------------------------------------------------
        [Fact]
        public void Reason_get_should_throw_exception_if_state_is_not_fulfilled()
        {
            Action action = () => { var val = new Promise().Reason; };

            action.ShouldThrow<InvalidOperationException>("it is not allowed to get the reason of a pending promise");
        }

        //---------------------------------------------------------------------
        [Fact]
        public void Value_get_should_return_the_correct_value_if_state_is_fulfilled()
        {
            var d = new Sailor();
            var promise = d.Promise;

            d.Resolve(3);

            d.Promise.Value.Should().Be(3, "a fulfilled promise value should return the parameter passed to the Sailor.Fulfill method");
        }

        //---------------------------------------------------------------------
        [Fact]
        public void Reason_get_should_return_the_reason_why_a_promise_has_been_rejected_if_state_is_rejected()
        {
            var d = new Sailor();
            var promise = d.Promise;

            var exc = new Exception("Exception");
            d.Reject(exc);

            d.Promise.Reason.Should().Be(exc, "a rejected promise reason should return the exception passed to the Sailor.Reject method");
        }

        //---------------------------------------------------------------------
        [Fact]
        public void Value_get_should_return_null_if_state_is_rejected()
        {
            var d = new Sailor();
            var promise = d.Promise;

            d.Reject(new Exception());

            d.Promise.Value.Should().BeNull("a rejected promise value should remain null");
        }

        //---------------------------------------------------------------------
        [Fact]
        public void Reason_get_should_return_null_if_state_is_fulfilled()
        {
            var d = new Sailor();
            var promise = d.Promise;

            d.Resolve(50);

            d.Promise.Reason.Should().BeNull("a fulfilled promise reason should remain null");
        }

        //---------------------------------------------------------------------
        [Fact]
        public void Then_method_should_return_a_new_promise()
        {
            var promise = new Promise();

            var promise1 = promise.Then(null);

            promise1.Should().NotBeSameAs(promise, "the Then method should return a new promise instance");
        }

        //---------------------------------------------------------------------
        [Fact]
        public void If_SynchronizationContext_is_called_then_the_new_promise_returned_by_the_Then_method_should_have_the_same_control()
        {
            var sc = new SynchronizationContext();

            var promise = new Promise();
            promise.SynchronizationContext = sc;

            var promise1 = promise.Then(null);

            promise.SynchronizationContext.Should().BeSameAs(sc, "the InvokeRequiredOn method should set that control");
            (promise1 as Promise).SynchronizationContext.Should().BeSameAs(promise.SynchronizationContext, "the Then method should return a new promise instance");
        }

        //---------------------------------------------------------------------
        [Fact]
        public void If_SynchronizationContext_is_called_then_the_new_promise_returned_by_the_Catch_method_should_have_the_same_control()
        {
            var sc = new SynchronizationContext();

            var promise = new Promise();
            promise.SynchronizationContext = sc;

            var promise1 = promise.OnError(null);

            promise.SynchronizationContext.Should().BeSameAs(sc, "the InvokeRequiredOn method should set that control");
            (promise1 as Promise).SynchronizationContext.Should().BeSameAs(promise.SynchronizationContext, "the Then method should return a new promise instance");
        }

        //---------------------------------------------------------------------
        [Fact]
        public void If_SynchronizationContext_is_called_then_the_new_promise_returned_by_the_Finally_method_should_have_the_same_control()
        {
            var sc = new SynchronizationContext();

            var promise = new Promise();
            promise.SynchronizationContext = sc;

            var promise1 = promise.Finally(null);

            promise.SynchronizationContext.Should().BeSameAs(sc, "the InvokeRequiredOn method should set that control");
            (promise1 as Promise).SynchronizationContext.Should().BeSameAs(promise.SynchronizationContext, "the Then method should return a new promise instance");
        }

        //---------------------------------------------------------------------
        [Fact]
        public void If_SynchronizationContext_is_called_then_the_new_promise_returned_by_the_Notify_method_should_have_the_same_control()
        {
            var sc = new SynchronizationContext();

            var promise = new Promise();
            promise.SynchronizationContext = sc;

            var promise1 = promise.Notify(null);

            promise.SynchronizationContext.Should().BeSameAs(sc, "the InvokeRequiredOn method should set that control");
            (promise1 as Promise).SynchronizationContext.Should().BeSameAs(promise.SynchronizationContext, "the Then method should return a new promise instance");
        }

        //---------------------------------------------------------------------
        [Fact]
        public void Same_promise_instance_Fulfill_action_sequence_call_should_respect_the_Then_calls_order()
        {
            DateTime? call1 = null;
            DateTime? call2 = null;

            object val = null;

            var fake1 = FakeItEasy.A.Fake<Fake>();
            FakeItEasy.A.CallTo(() => fake1.FakeAction(val))
                .WithAnyArguments()
                .Invokes(() => call1 = DateTime.Now);

            var fake2 = FakeItEasy.A.Fake<Fake>();
            FakeItEasy.A.CallTo(() => fake2.FakeAction(val))
                .WithAnyArguments()
                .Invokes(() => { Thread.Sleep(10); call2 = DateTime.Now; });

            var promise = new Promise();
            promise.Then(fake1.FakeAction);
            promise.Then(fake2.FakeAction);

            promise.Fulfill(new object());

            call1.Should().HaveValue();
            call2.Should().HaveValue();

            call1.Should().BeBefore(call2.Value, "the first promise fulfill method call should happen before the second promise fulfill method call");     
        }

        //---------------------------------------------------------------------
        [Fact]
        public void Same_promise_instance_Reject_action_sequence_call_should_respect_the_OnError_calls_order()
        {
            DateTime? call1 = null;
            DateTime? call2 = null;

            Exception exc = null;

            var fake1 = FakeItEasy.A.Fake<Fake>();
            FakeItEasy.A.CallTo(() => fake1.FakeAction(exc))
                .WithAnyArguments()
                .Invokes(() => call1 = DateTime.Now);

            var fake2 = FakeItEasy.A.Fake<Fake>();
            FakeItEasy.A.CallTo(() => fake2.FakeAction(exc))
                .WithAnyArguments()
                .Invokes(() => { Thread.Sleep(10); call2 = DateTime.Now; });

            var promise = new Promise();
            promise.OnError(fake1.FakeAction);
            promise.OnError(fake2.FakeAction);

            promise.Reject(new Exception());

            call1.Should().HaveValue();
            call2.Should().HaveValue();

            call1.Should().BeBefore(call2.Value, "the first promise reject method call should happen before the second promise reject method call");
        }

        //---------------------------------------------------------------------
        [Fact]
        public void Same_promise_instance_Notify_action_sequence_call_should_respect_the_Notify_calls_order()
        {
            DateTime? call1 = null;
            DateTime? call2 = null;

            object val = null;

            var fake1 = FakeItEasy.A.Fake<Fake>();
            FakeItEasy.A.CallTo(() => fake1.FakeAction(val))
                .WithAnyArguments()
                .Invokes(() => call1 = DateTime.Now);

            var fake2 = FakeItEasy.A.Fake<Fake>();
            FakeItEasy.A.CallTo(() => fake2.FakeAction(val))
                .WithAnyArguments()
                .Invokes(() => { Thread.Sleep(10); call2 = DateTime.Now; });

            var promise = new Promise();
            promise.Notify(fake1.FakeAction);
            promise.Notify(fake2.FakeAction);

            promise.Notify(new object());

            call1.Should().HaveValue();
            call2.Should().HaveValue();

            call1.Should().BeBefore(call2.Value, "the first promise notify method call should happen before the second promise notify method call");
        }

        //---------------------------------------------------------------------
        [Fact]
        public void Same_promise_instance_Finally_action_sequence_call_should_respect_the_Finally_calls_order()
        {
            DateTime? call1 = null;
            DateTime? call2 = null;

            var fake1 = FakeItEasy.A.Fake<Fake>();
            FakeItEasy.A.CallTo(() => fake1.FakeAction())
                .WithAnyArguments()
                .Invokes(() => call1 = DateTime.Now);

            var fake2 = FakeItEasy.A.Fake<Fake>();
            FakeItEasy.A.CallTo(() => fake2.FakeAction())
                .WithAnyArguments()
                .Invokes(() => { Thread.Sleep(10); call2 = DateTime.Now; });

            var promise = new Promise();
            promise.Finally(new Action(fake1.FakeAction));
            promise.Finally(new Action(fake2.FakeAction));

            promise.Finally();

            call1.Should().HaveValue();
            call2.Should().HaveValue();

            call1.Should().BeBefore(call2.Value, "the first promise finally method call should happen before the second promise finally method call");
        }

        //---------------------------------------------------------------------
        [Fact]
        public void Subsequent_promise_instances_fulfill_action_sequence_call_should_respect_the_Then_calls_order()
        {
            DateTime? call1 = null;
            var fake1 = FakeItEasy.A.Fake<Fake>();
            FakeItEasy.A.CallTo(() => fake1.FakeAction(null as object))
                .WithAnyArguments()
                .Invokes(() => call1 = DateTime.Now);

            DateTime? call2 = null;
            var fake2 = FakeItEasy.A.Fake<Fake>();
            FakeItEasy.A.CallTo(() => fake2.FakeAction(null as object))
                .WithAnyArguments()
                .Invokes(() => { Thread.Sleep(10); call2 = DateTime.Now; });

            DateTime? call3 = null;
            var fake3 = FakeItEasy.A.Fake<Fake>();
            FakeItEasy.A.CallTo(() => fake3.FakeAction(null as object))
                .WithAnyArguments()
                .Invokes(() => { Thread.Sleep(10); call3 = DateTime.Now; });

            var promise = new Promise();
            promise.Then(fake1.FakeAction)
                .Then(fake2.FakeAction)
                .Then(fake3.FakeAction);

            promise.Fulfill(null);

            call1.Should().HaveValue();
            call2.Should().HaveValue();
            call3.Should().HaveValue();

            call1.Should().BeBefore(call2.Value, "the first promise fulfill method call should happen before the second promise fulfill method call");
            call2.Should().BeBefore(call3.Value, "the second promise fulfill method call should happen before the third promise fulfill method call");
        }

        //---------------------------------------------------------------------
        [Fact]
        public void First_promise_rejected_should_reject_all_subsequent_promises()
        {
            bool call1 = false;
            bool call2 = false;
            bool call3 = false;

            var promise1 = new Promise();
            var promise2 = promise1.OnError((exc) => call1 = true);
            var promise3 = promise2.OnError((exc) => call2 = true);
            promise3.OnError((exc) => call3 = true);

            promise1.Reject(new Exception());

            call1.Should().BeTrue();
            call2.Should().BeTrue();
            call3.Should().BeTrue();
        }

        //---------------------------------------------------------------------
        [Fact]
        public void Subsequent_promise_instances_the_first_fulfill_but_the_second_reject_should_reject_all_promise_from_the_second_till_the_last()
        {
            object value = new object();
            var exc = new Exception();

            bool then1 = false;
            bool catch2 = false;
            bool catch3 = false;
            bool catch4 = false;

            var promise = new Promise();
            var promise1 = promise.Then((val) => then1 = true);
            var promise2 = promise1.Then((val) => { throw exc; }).OnError((e) => catch2 = true);
            var promise3 = promise2.Then((val) => { }).OnError((e) => catch3 = true);
            promise3.Then((val) => { }).OnError((e) => catch4 = true);

            promise.Fulfill(value);

            then1.Should().BeTrue();
            catch2.Should().BeTrue();
            catch3.Should().BeTrue();
            catch4.Should().BeTrue();
        }

        //---------------------------------------------------------------------
        [Fact]
        public void Fulfill_method_call_on_a_not_pending_promise_should_throw_InvalidOperationException()
        {
            var promise = new Promise();
            promise.Fulfill(new object());

            Action action = () => promise.Fulfill(new object());

            action.ShouldThrow<InvalidOperationException>("Cannot call fulfill method on a not pending promise");
        }

        //---------------------------------------------------------------------
        [Fact]
        public void Reject_method_call_on_a_not_pending_promise_should_throw_InvalidOperationException()
        {
            var promise = new Promise();
            promise.Reject(null);

            Action action = () => promise.Reject(null);

            action.ShouldThrow<InvalidOperationException>("Cannot call reject method on a not pending promise");
        }

        //---------------------------------------------------------------------
        [Fact]
        public void Catch_throwing_exception_should_not_prevent_the_catch_call_on_subsequent_promises()
        {
            bool call = false;

            var promise = new Promise();
            var promise1 = promise.OnError((exc) => { throw new Exception(); });
            promise1.OnError((exc) => call = true);

            Action action = () => promise.Reject(null);

            action.ShouldNotThrow<InvalidOperationException>("Catch throwing exception should not prevent the catch call on subsequent promises");
            call.Should().BeTrue();
        }

        //---------------------------------------------------------------------
        [Fact]
        public void Finally_throwing_exception_should_not_prevent_the_finally_call_on_subsequent_promises()
        {
            bool call = false;

            var promise = new Promise();
            var promise1 = promise.Finally(() => { throw new Exception(); });
            promise1.Finally(() => call = true);

            Action action = promise.Finally;

            action.ShouldNotThrow<InvalidOperationException>("Finally throwing exception should not prevent the finally call on subsequent promises");
            call.Should().BeTrue();
        }

        //---------------------------------------------------------------------
        [Fact]
        public void Notify_throwing_exception_should_not_prevent_the_notify_call_on_subsequent_promises()
        {
            bool call = false;

            var promise = new Promise();
            var promise1 = promise.Notify((obj) => { throw new Exception(); });
            promise1.Notify((obj) => call = true);

            Action action = () => promise.Notify(null as object);

            action.ShouldNotThrow<InvalidOperationException>("Notify throwing exception should not prevent the notify call on subsequent promises");
            call.Should().BeTrue();
        }
    }

    //=========================================================================
    public class Fake
    {
        public virtual void FakeAction(object obj)
        { }

        public virtual void FakeAction()
        { }

        public virtual void FakeAction(Exception exc)
        { }
    }
}
