using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Homicide.AI.Tasks
{
	public abstract class Act
	{
		public enum Status
		{
			Running,
			Fail,
			Success
		}

		public List<Act> Children { get; set; }

		public string name = "Act";
		public bool IsInitialized { get; set; }

		public bool IsCanceled { get; set; }

		[JsonIgnore]
		public IEnumerator<Status> enumerator;

		[JsonIgnore]
		public Act LastTickedChild { get; set; }

		public Act()
		{
			IsInitialized = false;
			Children = new();
		}

		public Status Tick()
		{
			if (enumerator == null)
				Initialize();

			if (enumerator != null)
				enumerator.MoveNext();
			else
				return Status.Fail;

			return enumerator.Current;
		}

		public virtual void Initialize()
		{
			enumerator = Run().GetEnumerator();
			IsInitialized = true;
		}

		public virtual IEnumerable<Status> Run() 
		{
			throw new NotImplementedException();
		}

		public virtual void OnCanceled()
		{
			IsCanceled = true;

			if (Children != null)
				foreach (Act child in Children)
					child.OnCanceled();
		}
	}
}
