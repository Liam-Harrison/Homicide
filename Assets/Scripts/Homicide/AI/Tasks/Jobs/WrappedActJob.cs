
namespace Homicide.AI.Tasks.Jobs
{
	/// <summary>
	/// A simple job which contains some provided act. The job will carry out the provided act.
	/// </summary>
	public class WrappedActJob : Job
	{
		private Act act;

		public WrappedActJob()
		{

		}

		public WrappedActJob(Act act)
		{
			this.act = act;
			name = act.name;
		}

		public override Feasibility IsFeasible(Agent agent)
		{
			return act != null ? Feasibility.Feasible : Feasibility.Infeasible;
		}

		public override bool ShouldDelete(Agent agent)
		{
			if (act == null)
				return true;
			return base.ShouldDelete(agent);
		}

		public override bool IsComplete()
		{
			if (act == null)
				return true;
			return base.IsComplete();
		}

		public override bool ShouldRetry(Agent agent)
		{
			return false;
		}

		public override Act CreateScript(Agent agent)
		{
			if (act != null)
				act.Initialize();
			return act;
		}
	}
}
