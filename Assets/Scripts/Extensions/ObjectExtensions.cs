using System;
using System.Collections.Generic;

namespace Homicide
{
	/// <summary>
	/// Contains a collection of object extensions.
	/// </summary>
	public static class ObjectExtensions
	{
		public static bool TryGetNumber(this object obj, out int value)
		{
			return int.TryParse(obj.ToString(), out value);
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            var rng = new Random();
            var n = list.Count;

            while (n > 1)
            {
                n--;
                var k = rng.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }
    }
}
