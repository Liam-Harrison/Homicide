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

			var brief = interactable.GetInteractionBrief;

			interactiveImage.sprite = brief.icon;
			interactiveLabel.text = brief.message;

			interactiveImage.gameObject.SetActive(brief.icon != null);
			interactiveLabel.gameObject.SetActive(true);
		}
	}
}