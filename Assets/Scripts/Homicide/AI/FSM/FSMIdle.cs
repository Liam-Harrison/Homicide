using UnityEngine;

namespace Homicide.AI.FSM
{
	public class FSMIdle : FSMState
	{
		private const float UpdateRate = 1;

		private float lastplan = 0;

		private bool planning = false;

		public override void EnterState(Agent agent)
		{
			planning = false;
		}

		public override void ExitState(Agent agent)
		{

		}

		public override void UpdateState(Agent agent)
		{
			if (!planning && Time.time > lastplan + UpdateRate)
				Plan(agent);
		}
		
		private async void Plan(Agent agent)
		{
			planning = true;
			lastplan = Time.time;
		}
	}
}