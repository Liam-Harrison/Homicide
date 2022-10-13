using Sirenix.OdinInspector;
using UnityEngine;

namespace Homicide.AI
{
	public class HumanAgent : Agent
	{
		[TabGroup("Locomotion")]
		public AudioClip[] footstepAudioClips;
		[Range(0, 1)] public float footstepAudioVolume = 0.5f;
		
		protected override void Start()
		{
			AgentAI = new HumanAI(this);
			base.Start();
		}

		private void OnFootstep(AnimationEvent animationEvent)
		{
			if (!(animationEvent.animatorClipInfo.weight > 0.5f)) return;
            
			var index = Random.Range(0, footstepAudioClips.Length);
			AudioSource.PlayClipAtPoint(footstepAudioClips[index], transform.position, footstepAudioVolume);
		}
	}
}
