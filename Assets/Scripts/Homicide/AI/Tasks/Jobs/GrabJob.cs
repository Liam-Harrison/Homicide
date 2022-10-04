
using Homicide.AI.Tasks.Acts;

namespace Homicide.AI.Tasks.Jobs
{
	public class GrabJob : Job
	{
		private Entity entity;

		public GrabJob(Entity entity)
		{
			name = $"Grab {entity.name} {entity.transform.position}";
			Category = TaskCategory.Other;
			Priority = TaskPriority.Medium;

			this.entity = entity;
		}

		public override Act CreateScript(Agent agent)
		{
			return new Grab(entity, agent.AgentAI);
		}

		public override bool IsComplete()
		{
			return entity == null;
		}
	}
}
