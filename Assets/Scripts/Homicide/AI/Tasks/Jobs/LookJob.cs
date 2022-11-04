
using Homicide.AI.Behaviours;
using Homicide.AI.Tasks.Acts;
using UnityEngine;

namespace Homicide.AI.Tasks.Jobs
{
	public class LookJob : Job
	{
		private LookPoint point;

		public LookJob(LookPoint point)
		{
			name = $"Look {point.name} {point.transform.position}";
			Category = TaskCategory.Other;
			Priority = TaskPriority.Medium;
			
			this.point = point;
		}

		public override Act CreateScript(Agent agent)
		{
			return new Look(point, agent.AgentAI);
		}

		public override bool IsComplete()
		{
			return false;
		}

		public override float ComputeCost(Agent agent, bool alreadyCheckedFeasible = false)
		{
			return -Vector3.Distance(agent.transform.position, point.transform.position);
		}
	}
}
