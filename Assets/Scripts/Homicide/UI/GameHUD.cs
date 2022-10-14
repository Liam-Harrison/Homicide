using Homicide.Game;
using Homicide.Game.Controllers;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Homicide.UI
{
	public class GameHUD : GameBehaviour
	{
        [TabGroup("Assignments"), Title("Interactable"), SerializeField]
		private Image interactiveImage;

		[TabGroup("Assignments"), SerializeField]
		private TextMeshProUGUI interactiveLabel;

		private void Awake()
		{
			ThirdPersonController.OnChangedInteractable += OnChangedInteractable;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			ThirdPersonController.OnChangedInteractable -= OnChangedInteractable;
		}

		private void OnChangedInteractable(IInteractable interactable)
		{
			if (interactable == null)
			{
				interactiveImage.gameObject.SetActive(false);
				interactiveLabel.gameObject.SetActive(false);
				return;
			}

			interactiveImage.sprite = interactable.GetInteractionBrief.Icon;
			interactiveLabel.text = interactable.GetInteractionBrief.Message;

			interactiveImage.gameObject.SetActive(true);
			interactiveLabel.gameObject.SetActive(true);
		}
	}
}