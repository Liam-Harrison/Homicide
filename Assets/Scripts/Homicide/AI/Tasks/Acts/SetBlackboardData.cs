using System.Collections.Generic;

namespace Homicide.AI.Tasks.Acts
{
	public class SetBlackboardData<T> : AgentAct
	{
		private string DataKey { get ; set; }
		private T Value { get; set; }

		public SetBlackboardData(AgentAI agent, string data, T value) : base(agent)
		{
			name = "Set " + data;
			DataKey = data;
			Value = value;
		}

		public override IEnumerable<Status> Run()
		{
			if (DataKey == null)
			{
				yield return Status.Fail;
			}
			else
			{
				AgentAI.Blackboard.SetData(DataKey, Value);
				yield return Status.Success;
			}
		}
	}
}
