using System;
using System.Collections.Generic;
using System.Linq;
using Homicide.AI.Tasks;
using Homicide.AI.Tasks.Acts;
using Homicide.AI.Tasks.Control_Flow;
using Homicide.AI.Tasks.Jobs;
using UnityEngine;

namespace Homicide.AI
{
	public class AgentAI
	{
		protected class IdleJob
		{
			public string name;
			public float chance = 1f;
			public Func<AgentAI, Job> create = (_) => null;
			public Func<AgentAI, bool> avaliable = (a) => true;
		}

		private readonly float updateRate = 2f;

		public bool ExecutingAction { get; private set; }

		public Agent Agent { get; private set; }

		public Job CurrentJob { get; private set; }

		public Act CurrentAct { get; private set; }

		public Blackboard Blackboard { get; private set; } = new();

		public readonly List<Job> jobs = new();

		public string LastFailedAct { get; private set; }

		public string LastTaskFailureReason { get; private set; }

		public float LastUpdate { get; private set; }

		protected static List<IdleJob> genericIdleTasks;

		public AgentAI(Agent agent)
		{
			Agent = agent;
			SetupIdleTasks();
		}

		public virtual void Update()
		{
			if (CurrentJob != null)
			{
				if (CurrentAct == null) return;
				
				try
				{
					var status = CurrentAct.Tick();
					var retried = false;

					if (CurrentAct != null)
					{
						if (status == Act.Status.Fail)
						{
							LastFailedAct = CurrentAct.name;

							if (CurrentJob.ShouldRetry(Agent) && jobs.Contains(CurrentJob) == false)
							{
								ReassignCurrentJob();
								retried = true;
							}
						}
					}

					if (CurrentJob.IsComplete() || CurrentJob.wasCancelled)
						ChangeJob(null);
					else if (status != Act.Status.Running && retried == false)
						ChangeJob(null);

				}
				catch (Exception e)
				{
					Debug.LogError($"Agent threw exception in act '{CurrentAct.name}'", Agent.gameObject);
					Debug.LogException(e);
					ChangeJob(null);
				}
			}
			else if (Time.time > LastUpdate + updateRate)
			{
				LastUpdate = Time.time;
				var job = TaskManager.Instance.GetBestTask(this);
				if (job != null)
				{
					ChangeJob(job);
				}
				else
				{
					OnIdle();
				}
			}
		}

		protected virtual void OnIdle()
		{
			var job = GetIdleJob(genericIdleTasks);
			ChangeJob(job);
		}

		protected void ChangeAct(Act act)
		{
			if (CurrentAct != null)
				CurrentAct.OnCanceled();

			CurrentAct = act;
		}

		public void ClearCurrentJob()
		{
			ChangeJob(null);
		}

		public void AssignJob(Job job)
		{
			if (job == null || jobs.Contains(job)) return;
			
			jobs.Add(job);
			job.OnAssign(this);
		}

		public void RemoveJob(Job job)
		{
			if (ReferenceEquals(job, CurrentJob))
				ClearCurrentJob();
		}

		public void ChangeJob(Job job)
		{
			if (CurrentJob != null)
				CurrentJob.OnUnAssign(this);

			CurrentJob = job;

			if (CurrentJob != null)
			{
				ChangeAct(CurrentJob.CreateScript(Agent));

				if (jobs.Contains(job))
					jobs.Remove(job);
				else
					job.OnAssign(this);
			}
			else
			{
				ChangeAct(null);
			}
		}

		public void ReassignCurrentJob()
		{
			if (CurrentJob == null) return;
			
			var job = CurrentJob;
			ChangeJob(null);
			AssignJob(job);
		}

		public int CountFeasibleJobs(TaskPriority priority)
		{
			return jobs.Count(job => job.Priority >= priority && job.IsFeasible(Agent) == Feasibility.Feasible);
		}

		private void SetTaskFailureReason(string message)
		{
			if (CurrentJob != null)
				CurrentJob.failureRecord.AddFailureReason(this, message);
			LastTaskFailureReason = message;
		}

		public IEnumerable<Act.Status> ClearBlackboardData(string data)
		{
			if (data == null)
			{
				SetTaskFailureReason("Failed to clear blackboard data because it was null.");
				yield return Act.Status.Fail;
			}
			else
			{
				Blackboard.Erase(data);
				yield return Act.Status.Success;
			}
		}

		protected Job GetIdleJob(params List<IdleJob>[] idleJobs)
		{
			var sum = idleJobs.Sum(l => l.Sum(j => j.chance));
			var r = UnityEngine.Random.Range(0, sum);

			foreach (var list in idleJobs)
			{
				foreach (var task in list)
				{
					if (r < task.chance)
						return task.create(this);
					r -= task.chance;
				}
			}

			return null;
		}

		private static void SetupIdleTasks()
		{
			if (genericIdleTasks != null)
				return;

			genericIdleTasks = new()
			{
				new()
				{
					name = "Wondering",
					chance = 1,
					create = (ai) => 
						new WrappedActJob(
							new WanderAct(ai)
							{
								name = "Wondering"
							})
						{
							autoRetry = false,
							name = "Wondering",
							Priority = TaskPriority.High,
						}
				},
				new()
				{
					name = "Standing Around",
					chance = 1,
					create = (ai) => 
						new WrappedActJob(
							new Wait(5)
							{
								name = "Standing Around"
							})
						{
							autoRetry = false,
							name = "Standing Around",
							Priority = TaskPriority.High,
						}
				}
			};
		}
	}
}
