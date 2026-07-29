using System;
using System.Collections.Generic;
using System.Linq;

namespace HarmonyLib
{
	// Token: 0x020001C3 RID: 451
	public static class CollectionExtensions
	{
		// Token: 0x060007DC RID: 2012 RVA: 0x00019D24 File Offset: 0x00017F24
		public static void Do<T>(this IEnumerable<T> sequence, Action<T> action)
		{
			if (sequence == null)
			{
				return;
			}
			foreach (T t in sequence)
			{
				action(t);
			}
		}

		// Token: 0x060007DD RID: 2013 RVA: 0x00019D52 File Offset: 0x00017F52
		public static void DoIf<T>(this IEnumerable<T> sequence, Func<T, bool> condition, Action<T> action)
		{
			sequence.Where<T>(condition).Do<T>(action);
		}

		// Token: 0x060007DE RID: 2014 RVA: 0x00019D61 File Offset: 0x00017F61
		public static IEnumerable<T> AddItem<T>(this IEnumerable<T> sequence, T item)
		{
			return (sequence ?? Array.Empty<T>()).Concat<T>(new T[] { item });
		}

		// Token: 0x060007DF RID: 2015 RVA: 0x00019D80 File Offset: 0x00017F80
		public static T[] AddToArray<T>(this T[] sequence, T item)
		{
			return sequence.AddItem(item).ToArray<T>();
		}

		// Token: 0x060007E0 RID: 2016 RVA: 0x00019D90 File Offset: 0x00017F90
		public static T[] AddRangeToArray<T>(this T[] sequence, T[] items)
		{
			List<T> list = new List<T>();
			list.AddRange(sequence ?? Enumerable.Empty<T>());
			list.AddRange(items);
			return list.ToArray();
		}

		// Token: 0x060007E1 RID: 2017 RVA: 0x00019DC0 File Offset: 0x00017FC0
		internal static Dictionary<K, V> Merge<K, V>(this IEnumerable<KeyValuePair<K, V>> firstDict, params IEnumerable<KeyValuePair<K, V>>[] otherDicts)
		{
			Dictionary<K, V> dictionary = new Dictionary<K, V>();
			foreach (KeyValuePair<K, V> keyValuePair in firstDict)
			{
				dictionary[keyValuePair.Key] = keyValuePair.Value;
			}
			foreach (IEnumerable<KeyValuePair<K, V>> enumerable in otherDicts)
			{
				foreach (KeyValuePair<K, V> keyValuePair2 in enumerable)
				{
					dictionary[keyValuePair2.Key] = keyValuePair2.Value;
				}
			}
			return dictionary;
		}

		// Token: 0x060007E2 RID: 2018 RVA: 0x00019E84 File Offset: 0x00018084
		internal static Dictionary<K, V> TransformKeys<K, V>(this Dictionary<K, V> origDict, Func<K, K> transform)
		{
			Dictionary<K, V> dictionary = new Dictionary<K, V>();
			foreach (KeyValuePair<K, V> keyValuePair in origDict)
			{
				dictionary.Add(transform(keyValuePair.Key), keyValuePair.Value);
			}
			return dictionary;
		}
	}
}
