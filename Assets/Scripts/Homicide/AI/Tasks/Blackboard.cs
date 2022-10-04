using System.Collections.Generic;
using Newtonsoft.Json;

namespace Homicide.AI.Tasks
{
	public class Blackboard
	{
		[JsonProperty]
		private Dictionary<string, IActData> data = new();

		public Blackboard()
		{
		}

		public void Clear()
		{
			data.Clear();
		}

		public bool Has(string key)
		{
			return data.ContainsKey(key);
		}

		public T GetData<T>(string key, T def)
		{
			if (data.ContainsKey(key) && data[key] is ActData<T> wrapper)
				return wrapper.Data;
			return def;
		}

		public T GetData<T>(string key)
		{
			if (data.ContainsKey(key) && data[key] is ActData<T> wrapper)
				return wrapper.Data;
			return default(T);
		}

		public void SetData<T>(string key, T value)
		{
			data[key] = new ActData<T>(value);
		}


		public void Erase(string key)
		{
			if (data.ContainsKey(key))
				data.Remove(key);
		}
	}
}