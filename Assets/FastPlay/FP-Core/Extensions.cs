using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FastPlay {
	public static class Extensions {

		public static bool IsInfinity(this float value) {
			return float.IsPositiveInfinity(value) || float.IsNegativeInfinity(value);
		}

		public static bool IsNaN(this float value) {
			return float.IsNaN(value);
		}

		public static bool IsNaN(this Vector2 value) {
			return value.x.IsNaN() || value.y.IsNaN();
		}

		public static bool IsNaN(this Vector3 value) {
			return value.x.IsNaN() || value.y.IsNaN() || value.z.IsNaN();
		}

		public static bool IsNaN(this Vector4 value) {
			return value.x.IsNaN() || value.y.IsNaN() || value.z.IsNaN() || value.w.IsNaN();
		}

		public static bool IsNaN(this Quaternion value) {
			return value.x.IsNaN() || value.y.IsNaN() || value.z.IsNaN() || value.w.IsNaN();
		}

		public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable) {
			if (enumerable == null) {
				return true;
			}
			var collection = enumerable as ICollection<T>;
			if (collection != null) {
				return collection.Count < 1;
			}
			return !enumerable.Any();
		}

		public static string NicifyPropertyName(this string input) {
			int index = 0;
			if (input.Contains("get_")) {
				index = input.IndexOf("get_");
				return (input.Substring(index, 4) + char.ToUpper(input[index + 4]) + input.Substring(index + 5)).Replace("get_", "Get");
			}
			else if (input.Contains("set_")) {
				index = input.IndexOf("set_");
				return (char.ToUpper(input[index + 4]) + input.Substring(index + 5)).Replace("set_", "Set");
			}
			return input;
		}

		public static string AddSpacesToSentence(this string input, bool first_char_to_upper = true, bool preserve_acronyms = true) {
			if (input.IsNullOrWhiteSpace()) return string.Empty;
			string new_text = string.Empty;
			if (first_char_to_upper) {
				new_text += char.ToUpper(input[0]);
			}
			else {
				new_text += input[0];
			}
			for (int id = 1; id < input.Length; id++) {
				if (char.IsUpper(input[id])) {
					if ((input[id - 1] != ' ' && !char.IsUpper(input[id - 1])) || (preserve_acronyms && char.IsUpper(input[id - 1]) && id < input.Length - 1 && !char.IsUpper(input[id + 1]))) {
						new_text += ' ';
					}
				}
				new_text += input[id];
			}
			return new_text;
		}

		public static string FirstCharToUpper(this string input) {
			if (input.IsNullOrEmpty()) return input;
			return char.ToUpper(input[0]) + input.Substring(1);
		}

		public static bool IsNullOrEmpty(this string input) {
			return (input == null || input.Length == 0);
		}

		public static bool IsNullOrWhiteSpace(this string input) {
			if (input == null) return true;

			for (int i = 0; i < input.Length; i++) {
				if (!Char.IsWhiteSpace(input[i])) return false;
			}

			return true;
		}

		public static T CastTo<T>(this object value) {
			if (value is T) {
				return (T)value;
			}
			return default(T);
		}

		public static void Resize(ref Array array, Type array_type, int new_size) {
			if (new_size < 0) {
				throw new ArgumentOutOfRangeException("new_size");
			}

			Array larray = array;
			if (larray == null) {
				array = Array.CreateInstance(array_type, new_size);
				return;
			}

			if (larray.Length != new_size) {
				Array new_array = Array.CreateInstance(array_type, new_size);
				Array.Copy(larray, 0, new_array, 0, larray.Length > new_size ? new_size : larray.Length);
				array = new_array;
			}
		}

		public static T[] AddItem<T>(this T[] array, T item) {
			List<T> list = new List<T>(array);
			list.Add(item);
			return list.ToArray<T>();
		}

		public static T[] RemoveItem<T>(this T[] array, T item) {
			List<T> list = new List<T>(array);
			list.Remove(item);
			return list.ToArray<T>();
		}

		public static List<T> MoveItem<T>(this List<T> list, int old_index, int new_index) {
			T item = list[old_index];
			list.RemoveAt(old_index);
			list.Insert(new_index, item);
			return list;
		}

		public static List<T> MoveItem<T>(this List<T> list, T item, int new_index) {
			int old_index = list.IndexOf(item);
			return MoveItem(list, old_index, new_index);
		}

		public static List<T> MoveItemToStart<T>(this List<T> list, T item) {
			return MoveItem(list, item, 0);
		}

		public static List<T> MoveItemToEnd<T>(this List<T> list, T item) {
			return MoveItem(list, item, list.Count - 1);
		}

		public static IEnumerable ToEnumerable(this IEnumerable enumerator) {
			while (enumerator.GetEnumerator().MoveNext()) {
				yield return enumerator.GetEnumerator().Current;
			}
		}

		public static IEnumerable<T> ToEnumerable<T>(this IEnumerator<T> enumerator) {
			while (enumerator.MoveNext()) {
				yield return enumerator.Current;
			}
		}

		public static void RenameKey<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey from_key, TKey to_key) {
			TValue v;
			if (dic.TryGetValue(to_key, out v)) return;
			TValue value = dic[from_key];
			dic.Remove(from_key);
			dic[to_key] = value;
		}
	}
}