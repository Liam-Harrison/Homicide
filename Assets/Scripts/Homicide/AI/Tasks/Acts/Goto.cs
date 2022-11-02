using System.Collections.Generic;
using Homicide.AI.Tasks.Control_Flow;
using Pathfinding;
using UnityEngine;

namespace Homicide.AI.Tasks.Acts
{
	public class Goto : CompoundAgentAct
	{
		public Vector3 position;

		public string blackboard;

		public bool failed;

		public bool computing;

		public bool traveling;

		public float maxTime;

		public Goto() { }

		public Goto(Vector3 pos, AgentAI agent) : base(agent)
		{
			position = pos;
			failed = false;
			traveling = false;
			computing = true;
			maxTime = Vector3.Distance(Agent.transform.position, pos) * 2.5f;
		}

		public Goto(string blackboard, AgentAI agent) : base(agent)
		{
			this.blackboard = blackboard;
			failed = false;
			traveling = false;
			computing = true;
		}

		public IEnumerable<Status> CheckPath()
		{
			while (computing)
				yield return Status.Running;

			if (failed == false)
				yield return Status.Success;
			else
				yield return Status.Fail;
		}

		public IEnumerable<Status> CheckStopped()
		{
			while (traveling)
				yield return Status.Running;

			yield return Status.Success;
		}

		private void PathFinished()
		{
			traveling = false;
		}

		private void PlanFinished(PathCompleteState state)
		{
			if (state == PathCompleteState.Error)
				failed = true;

			traveling = true;
			computing = false;
		}

		private bool StartPath()
		{
			if (string.IsNullOrEmpty(blackboard) == false)
				position = AgentAI.Blackboard.GetData<Vector3>(blackboard);

			Agent.AgentLocomotion.MoveTo(position, PlanFinished, PathFinished);
			return true;
		}

		private bool CheckSuccess()
		{
			return Vector3.Distance(Agent.transform.position, position) < 1f;
		}

		public override void Initialize()
		{
			Tree = new Sequence(
				new Do(StartPath),
				new Wrap(CheckPath),
				new Parallel(new Act[]{new Wrap(CheckStopped), new Wait(maxTime)}),
				new StopAct(AgentAI),
				new Do(CheckSuccess)
			);

			base.Initialize();
		}
	}
}
