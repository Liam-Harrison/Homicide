using System.Collections.Generic;
using UnityEngine;

namespace Homicide.AI.Tasks.Control_Flow
{
	public class Wait : Act
	{
        public float Elapsed { get; private set; }

        public float Duration { get; private set; }

        public bool Unscaled { get; private set; }

        public Wait(float time, bool unscaled = false)
        {
            name = "Wait " + time;
            Duration = time;
            Unscaled = unscaled;
        }

        public override void Initialize()
        {
            Elapsed = 0;
            base.Initialize();
        }

        public override IEnumerable<Status> Run()
        {
            LastTickedChild = this;

            while (Elapsed < Duration)
            {
                Elapsed += Unscaled ? Time.unscaledDeltaTime : Time.deltaTime;
                yield return Status.Running;
            }

            yield return Status.Success;
        }

    }
}
