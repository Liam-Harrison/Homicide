
namespace Homicide.AI.Tasks.Acts
{
	public class AgentAct : Act
	{
		public AgentAI AgentAI { get; set; }

		public Agent Agent => AgentAI.Agent;

		public AgentAct(AgentAI agent)
		{
			AgentAI = agent;
		}

		public AgentAct()
		{

		}
	}
}
