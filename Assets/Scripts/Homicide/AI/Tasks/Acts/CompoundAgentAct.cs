using System.Collections.Generic;

namespace Homicide.AI.Tasks.Acts
{
	public class CompoundAgentAct : AgentAct
	{
		protected Act Tree { get; set; }

		protected CompoundAgentAct(AgentAI agent) : base(agent)
		{
			name = "CompoundAgentAct";
			Tree = null;
		}

		protected CompoundAgentAct()
		{

		}

		public override void Initialize()
		{
			if (Tree != null)
			{
				Children.Clear();
				Tree.Initialize();
				Children.Add(Tree);
			}

			base.Initialize();
		}

		public override IEnumerable<Status> Run()
		{
			if (Tree == null)
				yield return Status.Fail;
			else
			{
				foreach (var s in Tree.Run())
				{
					LastTickedChild = Tree.LastTickedChild;
					yield return s;
				}
			}
		}
	}
}
