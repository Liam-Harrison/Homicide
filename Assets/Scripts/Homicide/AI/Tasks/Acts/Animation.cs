using System;
using System.Collections.Generic;
using UnityEngine;

namespace Homicide.AI.Tasks.Acts
{
    public class Animation : AgentAct
    {
        public int Parameter { get; private set; }

        public bool Value { get; private set; }
        
        public Animation(string parameter, bool value, AgentAI agent) : base(agent)
        {
            name = $"Set {parameter} to {value.ToString()}";
            Parameter =  Animator.StringToHash(parameter);
            Value = value;
        }

        public override IEnumerable<Status> Run()
        {
            Agent.Animator.SetBool(Parameter, Value);
            yield return Status.Success;
        }
    }
}