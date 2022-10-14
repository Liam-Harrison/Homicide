using Homicide.Game;
using System;
using UnityEngine;

[Serializable]
public class InteractionBrief
{
	[SerializeField] private string message;
	[SerializeField] private Sprite icon;

	public string Message => message;
	public Sprite Icon => icon;
}

public interface IInteractable
{
	public void OnInteracted(GameBehaviour source);
	public InteractionBrief GetInteractionBrief { get; }
}
