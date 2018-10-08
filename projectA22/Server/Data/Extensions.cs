﻿
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Data
{
	internal static class DictionaryExtensions
	{
		internal static TVal GetValueOrDefault<TKey, TVal>(this Dictionary<TKey, TVal> dict, TKey key) where TVal : class
		{
			TVal result = null;
			dict.TryGetValue(key, out result);
			return result;
		}

		internal static List<TVal> FindAll<TKey, TVal>(this Dictionary<TKey, TVal> dict, Func<KeyValuePair<TKey, TVal>, bool> predicate) where TVal : class
		{
			var result = new List<TVal>();

			foreach (var keyval in dict)
			{
				if (predicate(keyval))
					result.Add(keyval.Value);
			}

			return result;
		}
	}
}
