using System;
using System.Collections.Generic;

namespace Homicide.AI.Tasks.Control_Flow
{
	/// <summary>
	/// Wrap a coroutine which yields a <seealso cref="Act.Status"/>.
	/// </summary>
	public class Wrap : Act
	{
		private Func<IEnumerable<Status>> Function { get; set; }

		public Wrap(Func<IEnumerable<Status>> function)
		{
			name = function.Method.Name;
			Function = function;
		}

		public override void Initialize()
		{
			enumerator = Run().GetEnumerator();
		}

		public override IEnumerable<Status> Run()
		{
			LastTickedChild = this;
			foreach (var result in Function())
				yield return result;
		}
	}

	/// <summary>
	/// Performs the given function, if the function returns true this act will return the status
	/// Success, otherwise this act will return Fail.
	/// </summary>
	public class Do : Act
	{
		private Func<bool> Function { get; set; }

		public Do(Func<bool> function)
		{
			name = function.Method.Name;
			Function = function;
		}

		public override void Initialize()
		{
			enumerator = Run().GetEnumerator();
		}

		public override IEnumerable<Status> Run()
		{
			LastTickedChild = this;
			if (Function())
			{
				yield return Status.Success;
			}
			yield return Status.Fail;
		}
	}
}
