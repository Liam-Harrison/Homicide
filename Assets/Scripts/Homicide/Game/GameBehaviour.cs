using System;
using UnityEngine;

namespace Homicide.Game
{
    public abstract class GameBehaviour : MonoBehaviour
    {
        protected void Start()
        {
#if UNITY_EDITOR
            if (GetType().HasMethod("Update"))
                Debug.LogError("Do not use built-in Update method, use Game interface!");
            
            if (GetType().HasMethod("LateUpdate"))
                Debug.LogError("Do not use built-in LateUpdate method, use Game interface!");
#endif
            
            GameManager.Instance.Track(this);
        }
    }
}
