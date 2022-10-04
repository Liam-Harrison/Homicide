using System.Collections.Generic;
using Homicide.AI.Tasks.Acts;

namespace Homicide.AI.Tasks.Control_Flow
{
    /// <summary>
    /// Stops an agent.
    /// </summary>
	public class StopAct : AgentAct
    {
        public float StopForce { get; set; }

        public StopAct(AgentAI agent) :
            base(agent)
        {
            name = "Stop";
            StopForce = 10;
        }

        public override IEnumerable<Status> Run()
        {
            Agent.AgentLocomotion.StopLocomotion();
            yield return Status.Success;
        }
    }
}
