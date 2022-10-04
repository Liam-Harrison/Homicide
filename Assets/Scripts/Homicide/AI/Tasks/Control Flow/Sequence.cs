using System.Collections.Generic;
using System.Linq;

namespace Homicide.AI.Tasks.Control_Flow
{
	public class Sequence : Act
	{
        public int CurrentChildIndex { get; set; }

        public Act CurrentChild
        {
            get { return Children[CurrentChildIndex]; }
        }

        public Sequence()
        {

        }

        public Sequence(params Act[] children) : this(children.AsEnumerable())
        {

        }

        public Sequence(IEnumerable<Act> children)
        {
            name = "Sequence";
            Children = new();
            Children.AddRange(children);
            CurrentChildIndex = 0;
        }

        public override void Initialize()
        {
            CurrentChildIndex = 0;
            foreach (Act child in Children)
            {
                child.Initialize();
            }

            base.Initialize();
        }

        public override IEnumerable<Status> Run()
        {
            if (Children == null)
            {
                yield return Status.Fail;
                yield break;
            }

            while (CurrentChildIndex < Children.Count)
            {
                if (CurrentChild == null)
                {
                    yield return Status.Fail;
                    yield break;
                }

                Status childStatus = CurrentChild.Tick();
                LastTickedChild = CurrentChild;

                if (childStatus == Status.Fail)
                {
                    yield return Status.Fail;
                    yield break;
                }
                else if (childStatus == Status.Success)
                {
                    CurrentChildIndex++;
                }
                else
                {
                    yield return Status.Running;
                }
            }

            yield return Status.Success;
        }
    }
}
