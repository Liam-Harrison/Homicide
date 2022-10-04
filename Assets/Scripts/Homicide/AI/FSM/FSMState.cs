namespace Homicide.AI.FSM
{
	public abstract class FSMState
	{
		public abstract void EnterState(Agent agent);

		public abstract void UpdateState(Agent agent);

		public abstract void ExitState(Agent agent);
	}
}