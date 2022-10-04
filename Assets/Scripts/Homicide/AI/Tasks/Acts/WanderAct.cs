using Homicide.AI.Tasks.Control_Flow;
using UnityEngine;

namespace Homicide.AI.Tasks.Acts
{
	public class WanderAct : CompoundAgentAct
	{
		public WanderAct()
		{

		}

		public WanderAct(AgentAI agent) : base(agent)
		{

		}

		private bool SetRandomPosition()
		{
			float d = Random.Range(3, 7.5f);
			var o = Random.insideUnitSphere * d;
			var pos = Agent.transform.position + o;

			AgentAI.Blackboard.SetData("wonder_pos", pos);
			return true;
		}

		public override void Initialize()
		{
			Tree = new Sequence(
				new Do(SetRandomPosition),
				new Goto("wonder_pos", AgentAI)
			);
			base.Initialize();
		}
	}
}
