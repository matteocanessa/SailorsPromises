using System;
using System.Collections.Generic;
using System.Text;

namespace SailorsPromises
{
	/// <summary>
	/// A token to cancel a pending promise
	/// </summary>
	public class CancellationToken
	{
		/// <summary>
		/// true if a cancellation has been requested, otherwise false.
		/// </summary>
		public bool IsCancellationRequested { get; set; }
	}
}
