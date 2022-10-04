using System;
using System.Collections.Generic;
using UnityEngine;

namespace Homicide.Game
{
    public interface IUpdate
    {
        void GameUpdate();
    }

    public interface ILateUpdate
    {
        void GameLateUpdate();
    }
    
    public class GameManager : Singleton<GameManager>
    {
        private readonly HashSet<GameBehaviour> behaviours = new();
        private readonly HashSet<IUpdate> updates = new();
        private readonly HashSet<ILateUpdate> lateUpdates = new();
        
        private void Update()
        {
            foreach (var i in updates)
                i.GameUpdate();
        }

        private void LateUpdate()
        {
            foreach (var i in lateUpdates)
                i.GameLateUpdate();
        }

        public void Track(GameBehaviour behaviour)
        {
            if (behaviours.Contains(behaviour))
                return;
            
            behaviours.Add(behaviour);
            
            if (behaviour is IUpdate update)
                updates.Add(update);
            
            if (behaviour is ILateUpdate late)
                lateUpdates.Add(late);
        }

        public void Untrack(GameBehaviour behaviour)
        {
            if (!behaviours.Contains(behaviour))
                return;
            
            if (behaviour is IUpdate update)
                updates.Remove(update);
            
            if (behaviour is ILateUpdate late)
                lateUpdates.Remove(late);
            
            behaviours.Remove(behaviour);
        }
        
        public void UnloadBehaviours()
        {
            foreach (var behaviour in behaviours)
            {
                Destroy(behaviour.gameObject);
            }
        }
    }
}
