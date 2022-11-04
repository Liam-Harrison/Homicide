using Homicide.AI.Behaviours;
using Homicide.AI.Tasks.Control_Flow;

namespace Homicide.AI.Tasks.Acts
{
	public class Look : CompoundAgentAct
	{
		public LookPoint Point { get; set; }

		public Look() { }

		public Look(LookPoint point, AgentAI agent) : base(agent) 
		{
			Point = point;
		}
		
		public override void Initialize()
		{
			Tree = new Sequence(
				new Goto(Point.transform.position, AgentAI),
				new Animation("Investigating", true, AgentAI),
				new Wait(10f),
				new Animation("Investigating", false, AgentAI),
				new Do(() => false)
		);
			
		base.Initialize();
		}
	}
}
