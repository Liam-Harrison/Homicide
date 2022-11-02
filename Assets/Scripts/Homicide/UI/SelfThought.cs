using System;
using System.Collections;
using Homicide.Game;
using Homicide.Game.Controllers;
using TMPro;
using UnityEngine;

namespace Homicide.UI
{public class SelfThought : Singleton<SelfThought>
    {
        [SerializeField] private TextMeshProUGUI label;
        
        private RectTransform rect;
        
        protected override void Awake()
        {
            base.Awake();
            
            rect = GetComponent<RectTransform>();
            gameObject.SetActive(false);
        }
        
        public void ShowText(string text)
        {
            label.text = "";
            gameObject.SetActive(true);
            
            StopAllCoroutines();
            StartCoroutine(PrintText(text));
        }
        
        private void Update()
        {
            rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, 
                Camera.main.WorldToScreenPoint(ThirdPersonController.Instance.Follow.position),
                10 * Time.smoothDeltaTime);
        }
        
        private IEnumerator PrintText(string text)
        {
            foreach (var c in text)
            {
                label.text += c;
                yield return new WaitForSecondsRealtime(0.1f);
            }

            yield return new WaitForSecondsRealtime(5f);
            gameObject.SetActive(false);
        }
    }
}

