using System.Collections.Generic;
using System.Linq;
using Homicide.Game;
using Newtonsoft.Json;
using UnityEngine;

namespace Homicide.AI.Tasks
{
	public class TaskManager : Singleton<TaskManager>
	{
		[JsonProperty] private List<Job> jobs = new();

		public int JobCount => jobs.Count;

		public IEnumerable<Job> EnumerateTasks()
		{
			return jobs;
		}

		private void Update()
		{
			UpdateAgents();
		}

		public void AddJob(Job job)
		{
			if (jobs.Any(t => t.name == job.name)) return;
			
			jobs.Add(job);
			job.OnEnqueued();
		}

		public void AddJobs(IEnumerable<Job> tasks)
		{
			foreach (var task in tasks)
			{
				AddJob(task);
			}
		}

		public void CancelJob(Job job)
		{
			if (job == null)
				return;

			job.assigned.RemoveJob(job);
			jobs.RemoveAll(t => Object.ReferenceEquals(t, job));
			job.OnDequeued();
			job.OnCancelled(this);
			job.wasCancelled = true;
		}

		public Job GetBestTask(AgentAI agentAI, int minPriority = -1)
		{
			Job best = null;
			var bestCost = float.MaxValue;
			var bestPriority = TaskPriority.Eventually;
			var jobAgent = agentAI.Agent;
			
			foreach (var job in jobs)
			{
				if (job.assigned != null)
					continue;
				if (job.IsComplete())
					continue;
				if (job.IsFeasible(jobAgent) != Feasibility.Feasible)
					continue;
				if (job.Priority < bestPriority)
					continue;
				if ((int)job.Priority <= minPriority)
					continue;

				var cost = job.ComputeCost(jobAgent);

				if (cost < bestCost || job.Priority > bestPriority)
				{
					bestCost = cost;
					best = job;
					bestPriority = job.Priority;
				}

			}

			return best ?? null;
		}

		private int updateIdx = 0;
		private readonly int workGroupSize = 2048;

		public void UpdateAgents()
		{
			for (var k = 0; k < workGroupSize; k++)
			{
				var j = updateIdx + k;
				if (j >= jobs.Count)
				{
					updateIdx = 0;
					return;
				}

				jobs[j].CleanupInactiveWorkers();

				if (jobs[j].IsComplete())
				{
					jobs[j].OnDequeued();
					jobs.RemoveAt(j);
					k = Mathf.Max(k - 1, 0);
				}
				else
					jobs[j].OnUpdate();
			}

			updateIdx += workGroupSize;
		}

		public int GetMaxColumnValue(int[,] matrix, int column, int numRows, int numColumns)
		{
			var maxValue = int.MinValue;

			for (var r = 0; r < numRows; r++)
			{
				if (matrix[r, column] > maxValue)
				{
					maxValue = matrix[r, column];
				}
			}

			return maxValue;
		}

		public int GetMaxRowValue(int[,] matrix, int row, int numRows, int numColumns)
		{
			var maxValue = int.MinValue;

			for (var c = 0; c < numColumns; c++)
			{
				if (matrix[row, c] > maxValue)
				{
					maxValue = matrix[row, c];
				}
			}

			return maxValue;
		}

		private static int GetMax(int[,] matrix, int numRows, int numColumns)
		{
			var maxValue = int.MinValue;

			for (var c = 0; c < numColumns; c++)
			{
				for (var row = 0; row < numRows; row++)
				{
					if (matrix[row, c] > maxValue)
					{
						maxValue = matrix[row, c];
					}
				}
			}

			return maxValue;
		}

		public static List<Job> AssignTasksGreedy(List<Job> newGoals, List<AgentAI> agents, int maxPerAgent = 100, int maxToAssign = -1)
		{
			if (maxToAssign < 0)
			{
				maxToAssign = newGoals.Count;
			}
			// We are going to keep track of the unassigned goal count
			// to avoid having to parse the list at the end of the loop.
			var goalsUnassigned = newGoals.Count;

			// Randomized list changed from the CreatureAI objects themselves to an index into the
			// List passed in.  This is to avoid having to shift the masterCosts list around to match
			// each time we randomize.
			var randomIndex = new List<int>(agents.Count);
			for (var i = 0; i < agents.Count; i++)
			{
				randomIndex.Add(i);
			}

			// We create the comparer outside of the loop.  It gets reused for each sort.
			var toCompare = new CostComparer();

			// One of the biggest issues with the old function was that it was recalculating the whole task list for
			// the creature each time through the loop, using one item and then throwing it away.  Nothing changed
			// in how the calculation happened between each time so we will instead make a costs list for each creature
			// and keep them all.  This not only avoids rebuilding the list but the sheer KeyValuePair object churn there already was.
			var masterCosts = new List<List<KeyValuePair<int, float>>>(agents.Count);
			var creatureTaskCounts = new List<int>();

			// We will set this up in the next loop rather than make it's own loop.
			var costsPositions = new List<int>(agents.Count);
			foreach (var t in agents)
			{
				creatureTaskCounts.Add(t.CountFeasibleJobs(TaskPriority.Eventually));
				var costs = new List<KeyValuePair<int, float>>();
				var creature = t;

				// We already were doing an index count to be able to make the KeyValuePair for costs
				// and foreach uses Enumeration which is slower.
				for (var i = 0; i < newGoals.Count; i++)
				{
					var task = newGoals[i];
					// We are checking for tasks the creature is already assigned up here to avoid having to check
					// every task in the newGoals list against every task in the newGoals list.  The newGoals list
					// should reasonably not contain any task duplicates.
					if (creature.jobs.Contains(task)) continue;

					float cost = 0;
					// We've swapped the order of the two checks to take advantage of a new ComputeCost that can act different
					// if we say we've already called IsFeasible first.  This allows us to skip any calculations that are repeated in both.
					if (task.IsFeasible(creature.Agent) == Feasibility.Infeasible)
					{
						cost += 1e10f;
					}
					cost += task.ComputeCost(creature.Agent, true);
					cost += creature.jobs.Sum(existingTask => existingTask.ComputeCost(creature.Agent, true));
					costs.Add(new(i, cost));
				}
				// The sort lambda function has been replaced by an IComparer class.
				// This is faster but I mainly did it because VS can not Edit & Continue
				// any function with a Lambda function in it which was slowing down dev time.
				costs.Sort(toCompare);

				masterCosts.Add(costs);
				costsPositions.Add(0);
			}


			// We are going to precalculate the maximum iterations and count down
			// instead of up.
			var iterations = goalsUnassigned * agents.Count;
			var numAssigned = 0;
			while (goalsUnassigned > 0 && iterations > 0 && numAssigned < maxToAssign)
			{
				randomIndex.Shuffle();
				iterations--;
				foreach (var randomCreature in randomIndex)
				{
					var creature = agents[randomCreature];

					var costs = masterCosts[randomCreature];
					var costPosition = costsPositions[randomCreature];
					// This loop starts with the previous spot we stopped.  This avoids us having to constantly run a task we
					// know we have processed.
					for (var i = costPosition; i < costs.Count; i++)
					{
						// Incremented at the start in case we find a task and break.
						costPosition++;

						var taskCost = costs[i];
						// We've swapped the checks here.  Tasks.Contains is far more expensive so being able to skip
						// if it's going to fail the maxPerGoal check anyways is very good.
						if (!creature.jobs.Contains(newGoals[taskCost.Key]) &&
						    (creatureTaskCounts[randomCreature] < maxPerAgent || newGoals[taskCost.Key].Priority >= TaskPriority.High) &&
						    newGoals[taskCost.Key].IsFeasible(creature.Agent) == Feasibility.Feasible)
						{

							// We have to check to see if the task we are assigning is fully unassigned.  If so 
							// we reduce the goalsUnassigned count.  If it's already assigned we skip it.
							if (newGoals[taskCost.Key].assigned == null) goalsUnassigned--;

							creature.AssignJob(newGoals[taskCost.Key]);
							creatureTaskCounts[randomCreature]++;
							numAssigned++;
							break;
						}
					}
					// We have to set the position we'll start the loop at the next time based on where we found
					// our task.
					costsPositions[randomCreature] = costPosition;
				}

			}

			return newGoals.Where(t => t.assigned == null).ToList();
		}

		internal bool HasTask(Job task)
		{
			return jobs.Contains(task);
		}

		public static List<Job> AssignTasks(List<Job> newGoals, List<AgentAI> creatures, int maxPerDwarf = 100)
		{
			if (newGoals.Count == 0 || creatures.Count == 0)
			{
				return newGoals;
			}

			var unassignedGoals = new List<Job>();
			unassignedGoals.AddRange(newGoals);
			var numFeasible = 1;
			while (unassignedGoals.Count > 0 && numFeasible > 0)
			{
				numFeasible = 0;
				var assignments = CalculateOptimalAssignment(unassignedGoals, creatures);
				var removals = new List<Job>();
				for (var i = 0; i < creatures.Count; i++)
				{
					var assignment = assignments[i];

					if (assignment >= unassignedGoals.Count || creatures[i].Agent.IsDead
						|| unassignedGoals[assignment].IsFeasible(creatures[i].Agent) != Feasibility.Feasible ||
						creatures[i].CountFeasibleJobs(unassignedGoals[assignment].Priority) >= maxPerDwarf)
					{
						continue;
					}
					numFeasible++;
					creatures[i].AssignJob(unassignedGoals[assignment]);
					removals.Add(unassignedGoals[assignment]);
				}

				foreach (var removal in removals)
				{
					unassignedGoals.Remove(removal);
				}
			}
			return unassignedGoals;
		}

		private static int[] CalculateOptimalAssignment(List<Job> newGoals, List<AgentAI> agents)
		{
			var numGoals = newGoals.Count;
			var numAgents = agents.Count;
			var maxSize = Mathf.Max(numGoals, numAgents);

			var goalMatrix = new int[maxSize, maxSize];
			const float multiplier = 100;

			if (numGoals == 0 || numAgents == 0)
			{
				return null;
			}

			for (var goalIndex = 0; goalIndex < numGoals; goalIndex++)
			{
				var goal = newGoals[goalIndex];

				for (var agentIndex = 0; agentIndex < numAgents; agentIndex++)
				{
					var agent = agents[agentIndex];
					var floatCost = goal.ComputeCost(agent.Agent);

					var cost = (int)(floatCost * multiplier);

					if (goal.IsFeasible(agent.Agent) == Feasibility.Infeasible)
						cost += 99999;

					if (agent.Agent.IsDead)
						cost += 99999;

					cost += agents[agentIndex].jobs.Count;

					goalMatrix[agentIndex, goalIndex] = cost;
				}
			}

			// Add additional columns or rows
			if (numAgents > numGoals)
			{
				var maxValue = GetMax(goalMatrix, numAgents, numGoals) + 1;
				for (var dummyGoal = numGoals; dummyGoal < maxSize; dummyGoal++)
				{
					for (var i = 0; i < numAgents; i++)
					{
						// If we have more agents than goals, we need to add additional fake goals
						// Since goals are in columns, we are essentially adding a new column.
						goalMatrix[i, dummyGoal] = maxValue;
					}
				}
			}
			else if (numGoals > numAgents)
			{
				var maxValue = GetMax(goalMatrix, numAgents, numGoals) + 1;
				for (var agent = numAgents; agent < maxSize; agent++)
				{
					for (var i = 0; i < numGoals; i++)
					{
						// If we have more goals than agents, we need to add additional fake agents
						// Since goals are in columns, we are essentially adding a new row.
						goalMatrix[agent, i] = maxValue;
					}
				}
			}

			return goalMatrix.FindAssignments();
		}
		
		private class CostComparer : IComparer<KeyValuePair<int, float>>
		{
			public int Compare(KeyValuePair<int, float> pairA, KeyValuePair<int, float> pairB)
			{
				if (pairA.Key == pairB.Key)
					return 0;
				else 
					return pairA.Value.CompareTo(pairB.Value);
			}
		}
	}
}