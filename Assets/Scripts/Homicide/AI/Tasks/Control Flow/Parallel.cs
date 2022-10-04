using System.Collections.Generic;
using System.Linq;

namespace Homicide.AI.Tasks.Control_Flow
{
	/// <summary>
	/// Ticks all children in order each time this act is ticked. By default returns success when all children have 
	/// succeeded, this can be changed to return upon the first occuring success by setting <seealso cref="ReturnOnAllSuccess"/> to false.
	/// </summary>
	public class Parallel : Act
	{
		public bool ReturnOnAllSuccess { get; set; }

		public Parallel(params Act[] children) : this(children.AsEnumerable())
		{
			ReturnOnAllSuccess = true;
		}

		public Parallel(IEnumerable<Act> children)
		{
			name = "Parallel";
			Children = new();
			Children.AddRange(children);
			ReturnOnAllSuccess = true;
		}

		public override void Initialize()
		{
			foreach (var child in Children)
			{
				child.Initialize();
			}

			base.Initialize();
		}

		public override IEnumerable<Status> Run()
		{
			bool allPassed = false;

			while (allPassed == false)
			{
				bool run = false;
				foreach (var child in Children)
				{
					var status = child.Tick();
					LastTickedChild = child;

					if (status == Status.Fail)
					{
						yield return Status.Fail;
						yield break;
					}
					else if (status != Status.Success)
					{
						run = true;
					}
					else
					{
						if (ReturnOnAllSuccess == false)
						{
							yield return Status.Success;
							yield break;
						}
					}
				}

				if (run == false)
				{
					allPassed = true;
				}
				else
				{
					yield return Status.Running;
				}
			}

			yield return Status.Success;
		}
	}
}
