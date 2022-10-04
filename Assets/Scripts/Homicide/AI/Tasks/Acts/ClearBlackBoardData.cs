using System.Collections.Generic;

namespace Homicide.AI.Tasks.Acts
{
	public class ClearBlackboardData : AgentAct
	{
		private string DataKey { get ; set; }

		public ClearBlackboardData(AgentAI agent, string data) : base(agent)
		{
			name = "Set " + data;
			DataKey = data;
		}

		public override IEnumerable<Status> Run()
		{
			return AgentAI.ClearBlackboardData(DataKey);
		}
	}
}
