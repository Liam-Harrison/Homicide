using Homicide.AI.Tasks.Control_Flow;

namespace Homicide.AI.Tasks.Acts
{
	public class Grab : CompoundAgentAct
	{
		public Entity entity;

		public Grab() { }

		public Grab(Entity entity, AgentAI agent) : base(agent) 
		{
			this.entity = entity;
		}

		private bool GrabAction()
		{
			entity.gameObject.Destroy();
			return true;
		}

		public override void Initialize()
		{
			Tree = new Sequence(
	new Goto(entity.transform.position, AgentAI),
				new Do(GrabAction)
			);

			base.Initialize();
		}
	}
}
