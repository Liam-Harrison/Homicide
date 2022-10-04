using Homicide.Game;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Homicide.AI
{
	public class Entity : GameBehaviour
	{
		/// <summary>
		/// Does this entity belong to the player?
		/// </summary>
		public bool IsPlayer { get; private set; }

#if UNITY_EDITOR
		[Button]
		private void Unload()
		{
			Destroy(gameObject);
		}
#endif
	}
}