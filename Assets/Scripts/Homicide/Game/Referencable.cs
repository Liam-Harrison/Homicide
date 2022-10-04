using System.Collections.Generic;
using UnityEngine;

namespace Homicide.Game
{
    public interface IReferencable
    {
        uint ReferenceId { get; set; }
    }

    public static class Referencable
    {
        private static readonly Dictionary<uint, IReferencable> Referencables = new();

        private static uint nextId;

        public static void Track(IReferencable referencable)
        {
            Referencables.Add(nextId, referencable);
            referencable.ReferenceId = nextId++;
        }

        public static void Untrack(IReferencable referencable)
        {
            if (Referencables.ContainsKey(referencable.ReferenceId))
                Referencables.Remove(referencable.ReferenceId);
        }

        public static void Clear()
        {
            Referencables.Clear();
        }

        public static T Find<T>(uint referenceId) where T : class, IReferencable
        {
            Referencables.TryGetValue(referenceId, out var value);

            if (value is T casted) return casted;
            
            Debug.LogError($"Referencable object does not exist, of type {typeof(T).Name} with ID {referenceId}");
            return default;
        }
        
        
    }
}
