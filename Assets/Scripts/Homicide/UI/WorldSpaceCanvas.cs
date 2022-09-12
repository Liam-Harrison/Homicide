using System;
using UnityEngine;

namespace Homicide.UI
{
    public class WorldSpaceCanvas : MonoBehaviour
    {
        [SerializeField] private Vector2 meters;
        [SerializeField] private Vector2 canvas;
        
        private void OnValidate()
        {
            var rect = GetComponent<RectTransform>();
            var scale = meters / canvas;
            transform.localScale = scale;
            rect.sizeDelta = canvas;
        }
    }
}
