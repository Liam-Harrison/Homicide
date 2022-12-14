using Homicide.AI.FSM;
using Homicide.AI.Tasks;
using Homicide.Game;
using Homicide.Game.Controllers;
using UnityEngine;

namespace Homicide.AI
{
	public class Agent : Entity, IReferencable, IUpdate, IInteractable
	{
		[SerializeField]
		private GameObject ragdoll;

		[SerializeField]
		private InteractionBrief brief;

		[SerializeField]
		private bool debug;

		public FSM.FSM FSM { get; private set; }

		public bool IsDead { get; set; }

		public AgentAI AgentAI { get; protected set; }

		public AgentLocomotion AgentLocomotion { get; private set; }

		public uint ReferenceId { get; set; }

		public bool Active { get; set; }

		public bool Dead { get; set; }

		public InteractionBrief GetInteractionBrief
		{
			get
			{
				if (ThirdPersonController.Instance.Equipped == Weapon.Knife)
					return new InteractionBrief { icon = brief.icon, message = "Stab" };
				else
					return new InteractionBrief{ icon = null };
			}
		}
		
		public Animator Animator { get; private set; }

		protected override void Start()
		{
			base.Start();

			Animator = GetComponent<Animator>();

			FSM = new(this);
			AgentLocomotion = new(this);

			if (AgentAI == null)
				AgentAI = new(this);

			FSM.SetState<FSMIdle>();
		}

		public void GameUpdate()
		{
			if (Dead)
				return;
			
			FSM.UpdateState();
			AgentLocomotion.Update();
			AgentAI.Update();
		}

		private void OnGUI()
		{
			if (!debug) return;
			
			var y = 8;
			Print($"FSM: {FSM.State}", ref y);
			Print("", ref y);

			if (AgentAI.CurrentJob != null)
			{
				Print($"Job: {AgentAI.CurrentJob.name}", ref y);
				Print($"Assigned: {AgentAI.CurrentJob.assigned}", ref y);
			}
			else if (AgentAI.LastTaskFailureReason != "")
			{
				Print($"Job Failure: {AgentAI.LastTaskFailureReason}", ref y);
				Print($"Act Failure: {AgentAI.LastFailedAct}", ref y);
			}

			Print($"Avaliable: {TaskManager.Instance.JobCount}", ref y);
			Print("", ref y);

			if (AgentAI.CurrentAct != null)
			{
				Print($"Act: {AgentAI.CurrentAct.name}", ref y);

				if (AgentAI.CurrentAct.LastTickedChild != null)
				{
					Print($"Last Ticked: {AgentAI.CurrentAct.LastTickedChild.name}", ref y);
				}

				Print("", ref y);
			}

			if (AgentLocomotion.TargetNode != null)
			{

				Print($"Locomotion: {AgentLocomotion.TargetNode.Value}", ref y);
			}
			else
			{
				Print($"Locomotion: Inactive", ref y);
			}

			Print($"Last Update: {Time.time - AgentAI.LastUpdate:0.0}s ago", ref y);
		}

		private static void Print(string line, ref int y)
		{
			GUI.Label(new Rect(16, 16 + y, 300, 22), line);
			y += 22;
		}

		public void OnInteracted(GameBehaviour source)
		{
			if (IsDead)
				return;

			if (ThirdPersonController.Instance.Equipped == Weapon.None)
				return;

			IsDead = true;
			GenerateRagdoll();
			Destroy(gameObject);
		}
		
		private void GenerateRagdoll()
		{
			var go = Instantiate(ragdoll);
			TransformExtensions.CopyTransformChildren(transform, go.transform);
			go.gameObject.SetActive(true);
		}
	}
}
