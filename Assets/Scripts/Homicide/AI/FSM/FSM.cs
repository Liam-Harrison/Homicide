using System;
using System.Collections.Generic;

namespace Homicide.AI.FSM
{
	public class FSM
	{
		private readonly Agent agent;

		private readonly Dictionary<Type, FSMState> states = new();

		public FSMState State { get; private set; }

		public FSM(Agent agent)
		{
			this.agent = agent;
		}

		public void SetState<T>() where T: FSMState
		{
			if (State != null && State.GetType() == typeof(T))
				return;

			if (State != null)
				State.ExitState(agent);

			if (!states.ContainsKey(typeof(T)))
			{
				State = Activator.CreateInstance<T>();
				states.Add(typeof(T), State);
			}
			else
			{
				State = states[typeof(T)];
			}

			State.EnterState(agent);
		}

		public void UpdateState()
		{
			if (State != null)
				State.UpdateState(agent);
		}
	}
}