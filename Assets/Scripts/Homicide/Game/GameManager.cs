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
        private readonly HashSet<GameBehaviour> _behaviours = new HashSet<GameBehaviour>();
        private readonly HashSet<IUpdate> _updates = new HashSet<IUpdate>();
        private readonly HashSet<ILateUpdate> _lateUpdates = new HashSet<ILateUpdate>();
        
        private void Update()
        {
            foreach (var i in _updates)
                i.GameUpdate();
        }

        private void LateUpdate()
        {
            foreach (var i in _lateUpdates)
                i.GameLateUpdate();
        }

        public void Track(GameBehaviour behaviour)
        {
            if (_behaviours.Contains(behaviour))
                return;
            
            _behaviours.Add(behaviour);
            
            if (behaviour is IUpdate update)
                _updates.Add(update);
            
            if (behaviour is ILateUpdate late)
                _lateUpdates.Add(late);
        }

        public void Untrack(GameBehaviour behaviour)
        {
            if (!_behaviours.Contains(behaviour))
                return;
            
            if (behaviour is IUpdate update)
                _updates.Remove(update);
            
            if (behaviour is ILateUpdate late)
                _lateUpdates.Remove(late);
            
            _behaviours.Remove(behaviour);
        }
        
        public void UnloadBehaviours()
        {
            foreach (var behaviour in _behaviours)
            {
                Destroy(behaviour.gameObject);
            }
        }
    }
}
