#SailorsPromises         ![SailorsPromises logo](https://raw.githubusercontent.com/matteocanessa/SailorsPromises/master/SailorsPromises.png)
[![Stories in Ready](https://badge.waffle.io/matteocanessa/sailorspromises.png?label=ready&title=Ready)](https://waffle.io/matteocanessa/sailorspromises)
[![Build status](https://travis-ci.org/matteocanessa/SailorsPromises.svg)](https://travis-ci.org/matteocanessa/SailorsPromises)

SailorsPromises is a small, free and open-source library for the .NET Framework to make asynchronous calls more friendly.
It is a promise/deferred implementation inspired by [AngularJS $q](https://docs.angularjs.org/api/ng/service/$q) and [Promises/A+](http://promises-aplus.github.io/promises-spec/).

It can be installed via [Nuget](https://www.nuget.org/packages/SailorsPromises/).

##Implementing your own deferred service proxy

	public class MySuperServiceProxy
	{
		public MySuperServiceProxy()
		{
		}
		
		public IPromise Run()
		{
			var sailor = A.Sailor();
			
			new Thread(
			()
			=>
			{
					try {
						while (true) {
							//My long execution calling the service on the Internet...	
							Thread.Sleep(1000);
							sailor.Notify("Something happened");
							Thread.Sleep(1000);
							object result = null;
							sailor.Resolve(result);
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

Simply get an `ISailor` from `A` factory object and return its `Promise`.
During the service logic execution use the `ISailor` instance to interact with the returned `IPromise` for exceptions, notifications and so on.

##Using the service proxy

	var mySuperServiceInstance = new MySuperServiceProxy();

	var servicePromise = mySuperServiceInstance.Run();

	servicePromise.Then((obj) => {/*if the service completes the execution,
					here we are...*/})

	servicePromise.OnError((exc) => {/*if exceptions are raised,
					here we can catch them all...*/});


##Other usage examples:

	            A.Sailor()
				.When(() => { /*doing some stuff...*/ })
				.Then((obj) => {/*if When completes, here we are...*/})
				.OnError((exc) => {/*if exceptions are raised, here we can catch them all...*/});
				
The `When` action is executed asynchronously and when it is completed then the `Then` action is called.
If something goes wrong then the `OnError` method will catch the exception.

Then actions can be chained:

	            A.Sailor()
				.When(() => { /*doing some stuff...*/ })
				.Then((obj) => {/*if When completes, here we are...*/})
				.Then((obj) => {/*if the first Then completes, here we are...*/})
				.Then((obj) => {/*if the second Then completes, here we are...*/})
				.OnError((exc) => {/*if exceptions are raised by any of the actions
									above, here we can catch them all...*/});

the asynchronous execution takes place in order: the first `Then ` action, the second one and so on. If an exception is thrown by any of the actions then the`OnError` method is called and the execution chain is stopped.

A `Finally` method is always called at the end of the execution chain both when the execution ends without errors and when there are errors (it works exaclty like the C# `finally` keyword).

A `Notification` method is called whenever there is a notification from the action executed by the `When` method.

	            A.Sailor()
				.When(() => { /*doing some stuff...*/ })
				.Then((obj) => {/*if When completes, here we are...})
				.Notification((obj) => {/*show notifications here...*/})
				.Finally(() => {/*cleanup code here...*/});

##Examples in Windows Forms

The `InvokeRequired` and `Invoke` management typical of a multithreaded Windows Form application is automatically managed:

	A.Sailor().When(
		()
		=>
		{
			Thread.Sleep(5000);
		}
		)
		/*You can update controls state with no worries about cross thread operations*/
		.Then((value) => label.Text = "Hello!")
		.OnError((exc) => MessageBox.Show(exc.ToString()));

the `When` action is executed on another thread and when it completes the `Then` action execution is pushed on the current form message pump avoiding cross thread operations issues.
The same happens for all other actions (`OnError`, `Finally` and `Notification`).

##How to implement a splash window

			var splash = new Splash();

			var sailor = A.Sailor();
			sailor
				.When(() =>
				{
					Thread.Sleep(3000);
					sailor.Notify("First heavy loading success");
					Thread.Sleep(3000);
					sailor.Notify("Now its time to load something else...");
					Thread.Sleep(3000);
					sailor.Notify("Application loaded!");

				})
				.Notify((obj) => splash.SetText(obj.ToString()))
				.Finally(() => splash.Dispose());


			splash.ShowDialog(this);

The splash window is modal and the application goes on loading stuff in background.
Notifications are sent from the background thread to the Splash form without worrying about cross thread operations issues.

See the SailorsPromisesTestApp for full examples.