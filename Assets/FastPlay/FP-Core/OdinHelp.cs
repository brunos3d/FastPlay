using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using OdinSerializer;

namespace FastPlay {
	public static class OdinHelp {

		public static string ToJson<T>(T value) {
			byte[] bytes = SerializationUtility.SerializeValue(value, DataFormat.Binary);
			return JsonUtility.ToJson(bytes.ToList(), false);
		}

		public static string ToJson<T>(T value, out List<Object> referencedUnityObjects) {
			byte[] bytes = SerializationUtility.SerializeValue(value, DataFormat.Binary, out referencedUnityObjects);
			return JsonUtility.ToJson(bytes.ToList(), false);
		}

		public static T FromJson<T>(string json) {
			byte[] bytes = JsonUtility.FromJson<List<byte>>(json).ToArray();
			return SerializationUtility.DeserializeValue<T>(bytes, DataFormat.Binary);
		}

		public static T FromJson<T>(string json, List<Object> referencedUnityObjects) {
			byte[] bytes = JsonUtility.FromJson<List<byte>>(json).ToArray();
			return SerializationUtility.DeserializeValue<T>(bytes, DataFormat.Binary, referencedUnityObjects);
		}
	}
}
