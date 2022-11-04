using System.Collections.Generic;

namespace Homicide.AI.Tasks.Control_Flow
{
	/// <summary>
	/// Run a child while a condition act either is running or succeeds, condition act is reinitalized each time it is checked.
	/// Returns failure if the child fails. Returns success if the condition is true and the child succeeds, all other times returns running.
	/// </summary>
	public class While : Act
	{
		public Act Child { get; private set; }
		public Act Condition { get; private set; }

		public While(Act child, Act condition)
		{
			name = $"While : {condition.name}";
			Child = child;
			Condition = condition;
		}

		public override void Initialize()
		{
			Children.Clear();
			Children.Add(Child);

			Child.Initialize();
			Condition.Initialize();

			base.Initialize();
		}

		public bool CheckCondition()
		{
			Condition.Initialize();
			var status = Condition.Tick();
			return status != Status.Fail;
		}

		public override IEnumerable<Status> Run()
		{
			bool failed = false;
			while (CheckCondition())
			{
				Child.Initialize();

				while (CheckCondition())
				{
					Status status = Child.Tick();
					LastTickedChild = Child;

					if (status == Status.Fail)
					{
						failed = true;
						yield return Status.Fail;
						break;
					}
					else if (status == Status.Success)
					{
						yield return Status.Running;
						break;
					}
					else
					{
						yield return Status.Running;
					}
				}

				if (failed)
				{
					break;
				}
			}

			if (failed)
			{
				yield return Status.Fail;
			}
			else
			{
				yield return Status.Success;
			}
		}
	}
}
