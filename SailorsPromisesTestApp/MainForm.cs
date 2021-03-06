﻿//-----------------------------------------------------------------------
// <copyright file="MainForm.cs" company="https://github.com/matteocanessa/SailorsPromises">
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

using SailorsPromises;
using System;
using System.Threading;
using System.Windows.Forms;

namespace SailorsPromisesTestApp
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//

			//Show a splash window
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
		}
		
		 private void button1_Click(object sender, EventArgs e)
		{
			A.Sailor()
				.When(delegate() { /*doing some stuff...*/ })
				.Then(delegate(object obj) {/*if When completes, here we are...*/})
				.OnError(delegate(Exception exc) {/*if exceptions are raised, here we can catch them...*/});
		}

		private void button3_Click(object sender, EventArgs e)
		{
			A.Sailor()
				.When(
					delegate()
					{
						Thread.Sleep(5000);
					})
				.Then(delegate(object value) { button3.Text = "Horay!";})
				.OnError(delegate(Exception exc) { MessageBox.Show(exc.ToString());});
		}
	}
}
