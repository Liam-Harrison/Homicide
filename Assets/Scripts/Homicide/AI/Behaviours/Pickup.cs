using Homicide.AI;
using Homicide.AI.Tasks;
using Homicide.AI.Tasks.Jobs;
using Homicide.Game;
using Sirenix.OdinInspector;
using UnityEngine;

public class Pickup : Entity, IInteractable
{
	[TabGroup("Pickup"), SerializeField]
	private InteractionBrief brief;

	[SerializeField, TabGroup("Pickup")]
	private bool canAiPickup;

	public InteractionBrief GetInteractionBrief => brief;

	protected override void Start()
	{
		base.Start();

		if (canAiPickup)
			TaskManager.Instance.AddJob(new GrabJob(this));
	}

	public void OnInteracted(GameBehaviour source)
	{
		Destroy(gameObject);
	}
}
