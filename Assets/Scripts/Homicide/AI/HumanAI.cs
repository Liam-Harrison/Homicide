using System.Collections.Generic;
using Homicide.AI.Tasks.Jobs;
using UnityEngine;

namespace Homicide.AI
{
	public class HumanAI : AgentAI
	{
		private static List<IdleJob> humanIdleJobs;

		public HumanAI(HumanAgent agent) : base(agent)
		{
			SetupIdleTasks();
		}

		public override void Update()
		{
			base.Update();
		}

		protected override void OnIdle()
		{
			var job = GetIdleJob(humanIdleJobs, genericIdleTasks);

			if (job != null)
				ChangeJob(job);
		}

		private static void SetupIdleTasks()
		{
			if (humanIdleJobs != null)
				return;

			humanIdleJobs = new();
		}
	}
}
