//The MIT License (MIT)
//
//Copyright (c) 2014 Matteo Canessa (matcanessa@gmail.com)
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

namespace SailormanPromisesTests
{
	/// <summary>
	/// Description of SailormanTestsSuite.
	/// </summary>
	public class SailormanTestsSuite
	{
		[Fact]
		public void Resolve_method_should_call_promise_fulfill_exactly_once()
		{
			string val = "iyhbiyhb";

			var promise = A.Fake<Promise>();
			A.CallTo(() => promise.Fulfill(val))
				.DoesNothing();

			var d = new Sailorman(promise);
			d.Resolve(val);

			A.CallTo(() => promise.Fulfill(val))
				.MustHaveHappened(Repeated.Exactly.Once);
		}

		[Fact]
		public void Reject_method_should_call_promise_reject_exactly_once()
		{
			var exc = new Exception();

			var promise = A.Fake<Promise>();
			A.CallTo(() => promise.Reject(exc))
				.DoesNothing();

			var d = new Sailorman(promise);
			d.Reject(exc);

			A.CallTo(() => promise.Reject(exc))
				.MustHaveHappened(Repeated.Exactly.Once);
		}

		[Fact]
		public void Finally_method_should_call_promise_finally_exactly_once()
		{
			int call = 0;

			var d = new Sailorman();
			d.Promise.Finally(() => { call++; });
			d.Finally();

			call.Should().Be(1, "Finally method should call promise finally exactly once");
		}

		[Fact]
		public void Notify_method_should_call_promise_notify_exactly_once()
		{
			string val = "iyhbiyhb";
			int call = 0;

			var d = new Sailorman();
			d.Promise.Notify((obj) => { call++; });
			d.Notify(val);

			call.Should().Be(1, "Notify method should call promise notify exactly once");
		}
		
		[Fact]
		public void When_method_should_run_the_Action_and_all_others_methods_async_on_another_thread()
		{
			string whenThreadName = null;
			string thenThreadName = null;
			string finallyThreadName = null;

			var d = new Sailorman();
			d.When(() => {Thread.Sleep(500);whenThreadName = Thread.CurrentThread.Name;})
				.Then((obj) => thenThreadName = Thread.CurrentThread.Name)
				.Finally(() => finallyThreadName = Thread.CurrentThread.Name);
			
			Thread.Sleep(1000);
			
			string currentThreadName = Thread.CurrentThread.Name;
			
			whenThreadName.Should().NotBeNull();
			thenThreadName.Should().NotBeNull();
			finallyThreadName.Should().NotBeNull();
				
			currentThreadName.Should().NotBe(whenThreadName, "When method should run the Action async on another thread");
			currentThreadName.Should().NotBe(thenThreadName, "Then method should run the Action async on another thread");
			currentThreadName.Should().NotBe(finallyThreadName, "Finallyhen method should run the Action async on another thread");
		}
		
		[Fact]
		public void When_method_should_call_Then_Action_when_the_Action_passed_to_it_comlpetes_without_errors()
		{
			var callNr = 0;
			var callNr1 = 0;

			var d = new Sailorman();
			d.When(() => Thread.Sleep(500)).Then((obj) => callNr++).OnError((exc) => callNr1++);
			
			Thread.Sleep(1000);
			
			callNr.Should().Be(1, "When method should call Then Action when the Action passed to it comlpetes without errors");
			callNr1.Should().Be(0);
		}
		[Fact]
		public void When_method_should_call_Finally_action_when_the_Action_passed_to_it_comlpetes_without_errors()
		{
			var callNr = 0;
			var callNr1 = 0;

			var d = new Sailorman();
			d.When(() => Thread.Sleep(500)).Finally(() => callNr++).OnError((exc) => callNr1++);
			
			Thread.Sleep(1000);
			
			callNr.Should().Be(1, "When method should call Finally action when the Action passed to it comlpetes without errors");
			callNr1.Should().Be(0);
		}
		
		[Fact]
		public void When_method_should_call_Finally_action_when_the_Action_passed_to_it_comlpetes_with_errors()
		{
			var callNr = 0;
			var callNr1 = 0;
			var callNr2 = 0;

			var d = new Sailorman();
			d.When(() => {Thread.Sleep(500); throw new Exception();}).Finally(() => callNr++).OnError((exc) => callNr1++).Then((obj) => callNr2++);
			
			Thread.Sleep(1000);
			
			callNr.Should().Be(1, "When method should call Finally action when the Action passed to it comlpetes with errors");
			callNr1.Should().Be(1);
			callNr2.Should().Be(0);
		}
		
		[Fact]
		public void Then_action_should_be_called_on_the_same_current_thread_if_when_method_is_used_with_a_synch_context()
		{
			var sc = new SynchronizationContext();
			SynchronizationContext.SetSynchronizationContext(sc);
			
			string currentThreadName = Thread.CurrentThread.Name;
				
			bool whenThread = false;
			bool thenThread = false;
			bool finallyThread = false;

			var d = new Sailorman();
			d.When(() => {Thread.Sleep(500); whenThread = Thread.CurrentThread.Name == currentThreadName;})
				.Then((obj) => thenThread = Thread.CurrentThread.Name == currentThreadName)
				.Finally(() => finallyThread = Thread.CurrentThread.Name == currentThreadName);
			
			Thread.Sleep(1000);
			
			whenThread.Should().BeFalse();
			thenThread.Should().BeTrue();
			finallyThread.Should().BeTrue();
		}
	}
}
