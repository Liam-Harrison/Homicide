using System.Collections.Generic;
using System.Linq;

namespace Homicide.AI.Tasks.Control_Flow
{
	/// <summary>
	/// Runs children in sequence until one succeeds or all fail. Returns success if any child succeeds, otherwise fail.
	/// </summary>
	public class Select : Act
	{
		public Select()
		{

		}

		public Select(params Act[] children) : this(children.AsEnumerable())
		{

		}

		public Select(IEnumerable<Act> children)
		{
			name = "Select";
			Children = new();
			Children.AddRange(children);
		}

		public override void Initialize()
		{
			foreach (Act child in Children)
				if (child != null)
					child.Initialize();

			base.Initialize();
		}

		public override IEnumerable<Status> Run()
		{
			int childIndex = 0;
			while (childIndex < Children.Count)
			{
				switch (Children[childIndex].Tick())
				{
					case Status.Fail:
						childIndex += 1;
						break;
					case Status.Running:
						yield return Status.Running;
						break;
					case Status.Success:
						yield return Status.Success;
						yield break;
				}
			}

			yield return Status.Fail;
		}
	}
}
