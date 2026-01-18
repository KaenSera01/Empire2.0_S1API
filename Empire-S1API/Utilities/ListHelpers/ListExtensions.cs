using Core.DebugHandler;
using MelonLoader;
using System;
using System.Collections.Generic;

namespace Empire.Utilities.ListHelpers
{
	public static class ListExtensions
	{
		public static T RandomElement<T>(this IList<T> list)
		{
			if (list == null || list.Count == 0)
			{
				DebugLogger.Log("Cannot select a random element from an empty list.");
				return default;
			}

			return list[UnityEngine.Random.Range(0, list.Count)];
		}

		public static T RandomOrDefault<T>(this IList<T> list)
		{
			if (list == null || list.Count == 0)
				return default;

			return list[UnityEngine.Random.Range(0, list.Count)];
		}
	}
}
