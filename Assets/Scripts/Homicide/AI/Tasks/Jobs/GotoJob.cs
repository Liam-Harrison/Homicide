using Homicide.AI.Tasks.Acts;
using UnityEngine;

namespace Homicide.AI.Tasks.Jobs
{
	public class GotoJob : Job
	{
		private Vector3 pos;

		public GotoJob(Vector3 pos)
		{
			Category = TaskCategory.Other;
			Priority = TaskPriority.Urgent;

			this.pos = pos;
		}

		public override Act CreateScript(Agent agent)
		{
			return new Goto(pos, agent.AgentAI);
		}
	}
}
