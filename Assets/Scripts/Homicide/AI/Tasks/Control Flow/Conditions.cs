using System;
using System.Collections.Generic;

namespace Homicide.AI.Tasks.Control_Flow
{
	/// <summary>
	/// Evalatue a boolean or boolean function to determine the success of the act, if true then the act 
	/// will return 'Success', otherwise 'Fail'.
	/// </summary>
	public class Condition : Act
	{
		private Func<bool> Function { get; set; }

		public Condition() { }

		public Condition(bool condition)
		{
			name = "Condition";
			Function = () => condition;
		}

		public Condition(Func<bool> condition)
		{
			name = "Condition: " + condition.Method.Name;
			Function = condition;
		}

		public override IEnumerable<Status> Run()
		{
			LastTickedChild = this;
			if (Function())
			{
				yield return Status.Success;
			}
			else
			{
				yield return Status.Fail;
			}
		}
	}

	/// <summary>
	/// Evalatue a boolean or boolean function to determine if the child act is run or not. If the
	/// condition becomes false while the child is running the result will be a 'Fail' and execution stops.
	/// </summary>
	public class Domain : Act
	{
		private Func<bool> Function { get; set; }
		public Act Child { get; set; }

		public Domain() { }

		public Domain(bool condition, Act child)
		{
			name = "Domain";
			Function = () => condition;
			Child = child;
		}

		public Domain(Func<bool> condition, Act child)
		{
			name = $"Domain: {condition.Method.Name}";
			Function = condition;
			Child = child;
		}

		public override void Initialize()
		{
			Child.Initialize();
			base.Initialize();
		}

		public override void OnCanceled()
		{
			Child.OnCanceled();
			base.OnCanceled();
		}

		public override IEnumerable<Status> Run()
		{
			LastTickedChild = this;
			while (true)
			{
				if (Function())
				{
					var childStatus = Child.Tick();
					LastTickedChild = Child;
					if (childStatus == Status.Running)
					{
						yield return Status.Running;
						continue;
					}

					yield return childStatus;
					yield break;
				}
				else
				{
					yield return Status.Fail;
				}
			}
		}
	}
}
