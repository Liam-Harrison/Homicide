using System.Collections.Generic;

namespace Homicide.AI.Tasks
{
	public class FailureRecord
	{
		public string infeasibleReason = "";
		public List<string> failureReasons = new();
		public const int ReasonsToKeep = 5;

		public void AddFailureReason(AgentAI ai, string reason)
		{
			failureReasons.RemoveAll(r => r.StartsWith(ai.Agent.ReferenceId.ToString()));
			failureReasons.Add($"{ai.Agent.ReferenceId.ToString()}: {reason}");
			if (failureReasons.Count > ReasonsToKeep)
				failureReasons.RemoveAt(0);
		}

		public string FormatTooltip()
		{
			var r = infeasibleReason;
			if (!string.IsNullOrEmpty(infeasibleReason)) r += "\n";
			r += string.Join("\n", failureReasons);
			return r;
		}
	}
}
