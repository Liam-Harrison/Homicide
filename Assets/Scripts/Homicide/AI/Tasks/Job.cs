using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Homicide.AI.Tasks
{
	public abstract class Job
	{
        public TaskCategory Category { get; set; }
        public TaskPriority Priority { get; set; }
        public AgentAI assigned;
        public bool hidden = false;
        public bool autoRetry = false;
        public string name;
        public bool wasCancelled = false;

        public FailureRecord failureRecord = new();

        [JsonIgnore] public object guiTag = null;

        public override int GetHashCode() { return name != null ? name.GetHashCode() : 0; }
        protected bool Equals(Job other) { return other != null && name == other.name; }
        public override bool Equals(object obj) { return obj is Job job && string.Equals(name, job.name); }

        public void CleanupInactiveWorkers()
        {
            if (!assigned.Agent.Active || assigned.Agent.Dead)
                assigned = null;
        }

        public void OnAssign(AgentAI agent) { assigned = agent; }
        
        public void OnUnAssign(AgentAI agent) { assigned = agent; }

        public abstract Act CreateScript(Agent agent);

        public virtual float ComputeCost(Agent agent, bool alreadyCheckedFeasible = false) { return 1.0f; }
        
        public virtual Feasibility IsFeasible(Agent agent) { return Feasibility.Feasible; }
        
        public virtual bool ShouldRetry(Agent agent) { return autoRetry; }
        
        public virtual bool ShouldDelete(Agent agent) { return false; }
        
        public virtual bool IsComplete() { return false; }
        
        public virtual void OnEnqueued() { }
        
        public virtual void OnDequeued() { }
        
        public virtual void OnUpdate() { }
        
        public virtual void OnCancelled(TaskManager manager) { }
    }
}