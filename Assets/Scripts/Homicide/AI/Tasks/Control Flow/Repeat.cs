using System.Collections.Generic;

namespace Homicide.AI.Tasks.Control_Flow
{
    /// <summary>
    /// Loop over the items for a fixed number of iterations, restarts and reinitalizes each iteration.
    /// </summary>
	public class Repeat : Act
    {
        public Act Child { get; set; }
        public int Iters { get; set; }
        public bool BreakOnSuccess { get; set; }

        public Repeat(Act child, int iters, bool breakOnSuccess)
        {
            name = "For(" + iters + ")";
            Iters = iters;
            Child = child;
            BreakOnSuccess = breakOnSuccess;
            if (Iters < 0)
            {
                Iters = int.MaxValue;
            }
        }

        public override void Initialize()
        {
            Children.Clear();
            Children.Add(Child);
            Child.Initialize();
            base.Initialize();
        }

        public override IEnumerable<Status> Run()
        {
            bool failed = false;
            for (int i = 0; i < Iters; i++)
            {
                Child.Initialize();

                while (true)
                {
                    Status childStatus = Child.Tick();
                    LastTickedChild = Child;
                    if (childStatus == Status.Fail)
                    {
                        failed = true;
                        yield return Status.Running;
                        break;
                    }
                    else if (childStatus == Status.Success)
                    {
                        failed = false;
                        yield return Status.Running;
                        break;
                    }
                    else
                    {
                        yield return Status.Running;
                    }
                }

                if (failed == false && BreakOnSuccess)
                {
                    break;
                }
            }

            if (failed)
            {
                yield return Status.Fail;
            }
            else
            {
                yield return Status.Success;
            }
        }
    }
}
