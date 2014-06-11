#SailorsPromises

SailorsPromises is a small, free and open-source library for the .NET Framework to make asynchronous calls more friendly.
It is a promise/deferred implementation inspired by [AngularJS $q](https://docs.angularjs.org/api/ng/service/$q) and [Promises/A+](http://promises-aplus.github.io/promises-spec/)

##Usage examples:

	new Sailor()
				.When(() => { /*doing some stuff...*/ })
				.Then((obj) => {/*if When completes, here we are...*/})
				.OnError((exc) => {/*if exceptions are raised, here we can catch them all...*/});
				
The `When` action is executed asynchronously and when it is completed then the `Then` action is called.
If something goes wrong then the `OnError` method will catch the exception.

Then actions can be chained:

	new Sailor()
				.When(() => { /*doing some stuff...*/ })
				.Then((obj) => {/*if When completes, here we are...*/})
				.Then((obj) => {/*if the first Then completes, here we are...*/})
				.Then((obj) => {/*if the second Then completes, here we are...*/})
				.OnError((exc) => {/*if exceptions are raised by any of the actions above, here we can catch them all...*/});

the asynchronous execution takes place in order: the first `Then ` action, the second one and so on. If an exception is thrown by any of the actions the n the`OnError` method is called and the execution chain is stopped.

A `Finally` method is always called at the end of the execution chain both when the execution ends without errors and when there are errors (it works exaclty like the C# `finally` keyword).

A `Notification` method is called whenever there is a notification from the action executed by the `When` method.

	new Sailor()
				.When(() => { /*doing some stuff...*/ })
				.Then((obj) => {/*if When completes, here we are...})
				.Notification((obj) => {/*show notifications here...*/})
				.Finally(() => {/*cleanup code here...*/});

##Examples in Windows Forms

The `InvokeRequired` and `Invoke` management typical of a multithreaded Windows Form application is automatically managed:

	var d = new Sailor();

	d.When(
		()
		=>
		{
			Thread.Sleep(5000);
		}
		)
		.Then((value) => label.Text = "Hello!")
		.OnError((exc) => MessageBox.Show(exc.ToString()));

the `When` action is executed on another thread and when it completes the `Then` action execution is pushed on the current form message pump avoiding cross thread operations issues.
The same happens for all other actions (`OnError`, `Finally` and `Notification`).


##Implementing your own deferred service

	public class MySuperService
	{
		public MySuperService()
		{
		}
		
		public IPromise Run()
		{
			var sailor = new Sailor();
			
			new Thread(
			()
			=>
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

Simply create a new `Sailor` instance and return its `Promise`.
During the service logic execution use the `Sailor` instance to interact with the returned `Promise` for exceptions, notifications and so on.

See the SailorsPromisesTestApp for full examples.