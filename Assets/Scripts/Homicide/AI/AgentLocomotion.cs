using System;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

namespace Homicide.AI
{
	public class AgentLocomotion
	{
		private static int speed = Animator.StringToHash("Speed");
		private static int motion = Animator.StringToHash("MotionSpeed");

		private readonly Queue<Vector3> nodes = new();

		private const float MaxSpeed = 3.5f;
		private const float WalkSpeed = 2.25f;
		
		public bool Running { get; set; }

		public Vector3? TargetNode { get; private set; }

		public Agent Agent { get; private set; }

		public Path Path { get; private set; }

		private Seeker seeker;

		private CharacterController controller;
		private Animator animator;

		private float velcoity;
		private float v;

		private Action<PathCompleteState> planFinishedCallback;
		private Action finishedCallback;

		public AgentLocomotion(Agent agent)
		{
			Agent = agent;
			seeker = Agent.GetComponent<Seeker>();
			controller = Agent.GetComponent<CharacterController>();
			animator = Agent.GetComponentInChildren<Animator>();
		}

		public void Update()
		{
			MoveLocomotion();
		}

		public void MoveTo(Vector3 target, Action<PathCompleteState> onPlanFinished = null, Action onFinished = null)
		{
			nodes.Clear();

			TargetNode = null;
			planFinishedCallback = onPlanFinished;
			finishedCallback = onFinished;

			seeker.StartPath(Agent.transform.position, target, OnPathComplete);
		}

		public void StopLocomotion()
		{
			velcoity = Mathf.SmoothDamp(velcoity, 0, ref v, 0.4f);

			var p = Mathf.Clamp01(velcoity / MaxSpeed);
			animator.SetFloat(speed, Mathf.Lerp(0, 6, p));
			animator.SetFloat(motion, p);
		}

		private void MoveLocomotion()
		{
			float targetVel = 0;

			if (TargetNode != null)
			{
				targetVel = Running ? MaxSpeed : WalkSpeed;

				var d = (TargetNode.Value - Agent.transform.position).normalized;
				controller.Move(d * (velcoity * Time.smoothDeltaTime) + new Vector3(0.0f, -6f, 0.0f) * Time.deltaTime);

				var fwd = d.RemoveAxes(y: true);

				if (fwd != Vector3.zero)
					Agent.transform.rotation = Quaternion.RotateTowards(Agent.transform.rotation, Quaternion.LookRotation(fwd), 360f * Time.smoothDeltaTime);

				if (Vector3.Distance(TargetNode.Value, Agent.transform.position) <= 0.5f)
				{
					if (nodes.Count > 0)
						TargetNode = nodes.Dequeue();
					else
					{
						TargetNode = null;

						finishedCallback?.Invoke();
						finishedCallback = null;
					}
				}
			}

			velcoity = Mathf.SmoothDamp(velcoity, targetVel, ref v, 0.4f);

			var p = Mathf.Clamp01(velcoity / MaxSpeed);
			animator.SetFloat(speed, Mathf.Lerp(0, 6, p));
			animator.SetFloat(motion, p);
		}

		private void OnPathComplete(Path path)
		{
			if (path.CompleteState != PathCompleteState.Error)
			{
				foreach (var node in path.vectorPath)
				{
					nodes.Enqueue(node);
				}
				TargetNode = nodes.Dequeue();
				Path = path;
			}

			planFinishedCallback?.Invoke(path.CompleteState);
			planFinishedCallback = null;
		}
	}
}
