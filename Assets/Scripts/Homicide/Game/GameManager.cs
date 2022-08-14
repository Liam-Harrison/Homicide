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

        public void Track(GameBehaviour item)
        {
            if (item is IUpdate update)
                _updates.Add(update);
            
            if (item is ILateUpdate late)
                _lateUpdates.Add(late);
        }
    }
}
