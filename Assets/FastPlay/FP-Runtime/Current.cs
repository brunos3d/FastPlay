using System;
using UnityEngine;

namespace FastPlay.Runtime {
	public static class Current {

		public static GameObject gameObject { get; set; }

		public static GraphAsset asset { get; set; }

		public static Graph graph { get; set; }

		public static GraphController controller { get; set; }

		public static Transform transform {
			get {
				if (!gameObject) return null;
				return gameObject.transform;
			}
		}

		public static void DebugInfo() {
			string message = string.Empty;
			message += "gameObject name: " + gameObject.name + "\n";
			message += "gameObject id: " + gameObject.GetInstanceID() + "\n";
			message += "graph name: " + graph.name + "\n";
			message += "graph id: " + graph.GetInstanceID() + "\n";
			message += "controller name: " + controller.name + "\n";
			message += "controller id: " + controller.GetInstanceID() + "\n";
			Debug.Log(message);
		}

		public static void SetCurrent(GraphController controller) {
			if (!controller) return;
			Current.controller = controller;
			Current.asset = controller.graph;
			Current.graph = controller.graph.graph;
			Current.gameObject = controller.gameObject;
		}

		public static VariableObject<T> FindVariable<T>(string name) {
			if (!controller) return default(VariableObject<T>);
			return controller.FindVariable<T>(name);
		}

		public static VariableObject GetVariable(int key) {
			if (!controller) return null;
			return controller.GetVariable(key);
		}

		public static VariableObject<T> GetVariable<T>(int key) {
			if (!controller) return default(VariableObject<T>);
			return controller.GetVariable<T>(key);
		}

		public static T GetComponent<T>() where T : Component {
			if (!gameObject) return null;
			return gameObject.GetComponent<T>();
		}

		public static Component GetComponent(Type type) {
			if (!gameObject) return null;
			return gameObject.GetComponent(type);
		}

		public static Component GetComponent(string type) {
			if (!gameObject) return null;
			return gameObject.GetComponent(type);
		}

		public static T GetComponentInParent<T>() where T : Component {
			if (!gameObject) return null;
			return gameObject.GetComponentInParent<T>();
		}

		public static Component GetComponentInParent(Type type) {
			if (!gameObject) return null;
			return gameObject.GetComponentInParent(type);
		}

		public static T GetComponentInChildren<T>(bool includeInactive) where T : Component {
			if (!gameObject) return null;
			return gameObject.GetComponentInChildren<T>(includeInactive);
		}

		public static Component GetComponentInChildren(Type type, bool includeInactive) {
			if (!gameObject) return null;
			return gameObject.GetComponentInChildren(type, includeInactive);
		}
	}
}
