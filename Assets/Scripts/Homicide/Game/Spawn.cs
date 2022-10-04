using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Homicide.Game
{
    public class Spawn : MonoBehaviour
    {
        [SerializeField, Title("Settings")] private GameObject prefab;
        
        private void Start()
        {
            if (prefab != null)
                Instantiate(prefab, transform.position, transform.rotation);
        }

        private void OnDrawGizmos()
        {
            if (prefab == null) return;
            
            Gizmos.color = Color.green;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.up * 0.9f, new Vector3(0.56f, 1.8f, 0.56f));
        }
    }
}
