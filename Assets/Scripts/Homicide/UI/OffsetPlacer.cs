using System;
using UnityEngine;

namespace Homicide.UI
{
    public class OffsetPlacer : MonoBehaviour
    {
        [SerializeField]
        private Vector3 offset;
        
        private void Awake()
        {
            offset = transform.localPosition;
        }

        private void Update()
        {
            var cross = Vector3.Cross(Camera.main.transform.forward, Vector3.up);
            var rot = Quaternion.LookRotation(cross) * new Vector3(offset.x, 0, offset.z);
            transform.position = transform.parent.position + rot + new Vector3(0, offset.y, 0);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawSphere(transform.position, 0.15f);
        }
    }
}