using Homicide.AI.Tasks;
using Homicide.AI.Tasks.Jobs;
using Homicide.Game;

namespace Homicide.AI.Behaviours
{
    public class LookPoint : GameBehaviour
    {
        protected override void Start()
        {
            base.Start();

            TaskManager.Instance.AddJob(new LookJob(this));
        }
        
    }
}
