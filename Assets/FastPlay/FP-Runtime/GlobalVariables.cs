using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using OdinSerializer;

namespace FastPlay.Runtime {
	public class GlobalVariables : SerializedMonoBehaviour {

		public Dictionary<int, VariableObject> properties = new Dictionary<int, VariableObject>();

		public VariableObject AddVariable(int key, string name, Type type) {
			VariableObject v;
			if (properties.TryGetValue(key, out v)) {
				v.name = name;
				return v;
			}
			VariableObject instance = (VariableObject)ObjectBase.CreateGenericInstance(typeof(VariableObject<>), type, name);
			properties[key] = instance;
			return instance;
		}

		public VariableObject<T> AddVariable<T>(int key, string name) {
			VariableObject v;
			if (properties.TryGetValue(key, out v)) {
				v.name = name;
				return (VariableObject<T>)v;
			}
			VariableObject<T> instance = ObjectBase.CreateInstance<VariableObject<T>>(name);
			properties[key] = instance;
			return instance;
		}

		public VariableObject<T> FindVariable<T>(string name) {
			return (VariableObject<T>)properties.Values.First(p => p.name == name);
		}

		public VariableObject<T> GetVariable<T>(int key) {
			Debug.Log(name + " : " + properties.Count);
			return (VariableObject<T>)properties[key];
		}
	}
}
