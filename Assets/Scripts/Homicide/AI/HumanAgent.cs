namespace Homicide.AI
{
	public class HumanAgent : Agent
	{
		protected override void Start()
		{
			AgentAI = new HumanAI(this);
			base.Start();
		}
	}
}
