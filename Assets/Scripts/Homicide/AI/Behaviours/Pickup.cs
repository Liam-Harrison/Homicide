using Homicide.AI.Tasks;
using Homicide.AI.Tasks.Jobs;
using Homicide.Game;
using Homicide.Game.Controllers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Homicide.AI.Behaviours
{
	public class Pickup : Entity, IInteractable
	{
		[TabGroup("Pickup"), SerializeField]
		private InteractionBrief brief;

		[SerializeField, TabGroup("Pickup")]
		private bool canAiPickup;
		
		[SerializeField, TabGroup("Pickup")]
		private Weapon weapon;

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
			
			if (source is ThirdPersonController player)
				player.Equip(weapon);
		}
	}
}
