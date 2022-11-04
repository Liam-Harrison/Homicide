using System;
using UnityEngine;

namespace Homicide.Game
{
	[Serializable]
	public class InteractionBrief
	{
		public string message;
		public Sprite icon;
	}

	public interface IInteractable
	{
		public void OnInteracted(GameBehaviour source);
		public InteractionBrief GetInteractionBrief { get; }
	}
}