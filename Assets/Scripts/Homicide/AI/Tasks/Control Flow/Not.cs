using System.Collections.Generic;

namespace Homicide.AI.Tasks.Control_Flow
{
	/// <summary>
	/// Inverts the results of the child act, it will return success on failure, and failure on success. If the child
	/// is running it will just return running.
	/// </summary>
	public class Not : Act
	{
		private Act Child { get; set; }

		public Not(Act child)
		{
			name = "Not";
			Child = child;
		}

		public override void Initialize()
		{
			Children.Clear();
			Children.Add(Child);
			Child.Initialize();
			base.Initialize();
		}

		public override IEnumerable<Status> Run()
		{
			while (true)
			{
				var status = Child.Tick();
				LastTickedChild = Child;

				if (status == Status.Running)
				{
					yield return Status.Running;
				}
				else if (status == Status.Success)
				{
					yield return Status.Fail;
					break;
				}
				else if (status == Status.Fail)
				{
					yield return Status.Success;
					break;
				}
			}
		}
	}
}
