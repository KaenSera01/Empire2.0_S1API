using System.Collections.Generic;
using UnityEngine;

namespace Empire.Utilities.DealDayHelpers
{
	public static class DealDayUtility
	{
		// Canonical day map: key = order, value = name
		public static readonly Dictionary<int, string> DaysOfTheWeek = new Dictionary<int, string>
		{
			{ 0, "Sunday" },
			{ 1, "Monday" },
			{ 2, "Tuesday" },
			{ 3, "Wednesday" },
			{ 4, "Thursday" },
			{ 5, "Friday" },
			{ 6, "Saturday" }
		};

		/// <summary>
		/// Returns a randomized list of unique day names, sorted by weekday order.
		/// </summary>
		public static List<string> GetRandomDays(int count)
		{
			count = Mathf.Clamp(count, 1, DaysOfTheWeek.Count);

			// Copy keys into a list so we can shuffle them
			List<int> keys = new List<int>(DaysOfTheWeek.Keys);

			// Fisher-Yates shuffle on keys
			for (int i = keys.Count - 1; i > 0; i--)
			{
				int j = UnityEngine.Random.Range(0, i + 1);
				int temp = keys[i];
				keys[i] = keys[j];
				keys[j] = temp;
			}

			// Take the first N keys, then sort them
			List<int> selectedKeys = keys.GetRange(0, count);
			selectedKeys.Sort();

			// Convert keys back to day names
			List<string> result = new List<string>();
			for (int i = 0; i < selectedKeys.Count; i++)
			{
				result.Add(DaysOfTheWeek[selectedKeys[i]]);
			}

			return result;
		}

		/// <summary>
		/// Returns the first N days in canonical order.
		/// </summary>
		public static List<string> GetSequentialDays(int count)
		{
			count = Mathf.Clamp(count, 1, DaysOfTheWeek.Count);

			List<string> result = new List<string>();

			for (int i = 0; i < count; i++)
				result.Add(DaysOfTheWeek[i]);

			return result;
		}

		/// <summary>
		/// Returns either random or sequential days depending on a flag.
		/// </summary>
		public static List<string> GetDealDays(int count, bool randomize)
		{
			return randomize
				? GetRandomDays(count)
				: GetSequentialDays(count);
		}
	}
}
