using System;
using UnityEngine;

namespace Homicide.Game
{
    public abstract class GameBehaviour : MonoBehaviour
    {
        protected virtual void Start()
        {
#if UNITY_EDITOR
            if (GetType().HasMethod("Update"))
                Debug.LogError("Do not use builtin Update method, use Game interface!", gameObject);
            
            if (GetType().HasMethod("LateUpdate"))
                Debug.LogError("Do not use builtin LateUpdate method, use Game interface!", gameObject);
#endif
            
            GameManager.Instance.Track(this);
        }

        protected virtual void OnDestroy()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.Untrack(this);
        }
    }
}
