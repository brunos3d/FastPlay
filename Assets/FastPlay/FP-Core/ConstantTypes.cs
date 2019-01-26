using System;
using System.Collections.Generic;
using UnityEngine;
using UnityObject = UnityEngine.Object;
using FastPlay.Runtime;
using OdinSerializer;

namespace FastPlay {
	public class ConstantTypes : ScriptableObject, ISerializationCallbackReceiver {

		public static List<Type> default_types = new List<Type>() {
			// System
		typeof(Type),
		typeof(bool),
		typeof(float),
		typeof(int),
		typeof(object),
		typeof(string),
		typeof(List<>),
			// FastPlay
		typeof(Graph),
		typeof(GraphAsset),
		typeof(GraphController),
		typeof(Node),
			// UnityEngine
		typeof(Animation),
		typeof(Animator),
		typeof(Application),
		typeof(AudioClip),
		typeof(AudioListener),
		typeof(AudioSource),
		typeof(BoxCollider),
		typeof(Camera),
		typeof(CharacterController),
		typeof(CharacterJoint),
		typeof(Cloth),
		typeof(Collider),
		typeof(Collider2D),
		typeof(Collision),
		typeof(Collision2D),
		typeof(Color),
		typeof(Component),
		typeof(Cursor),
		typeof(Debug),
		typeof(GameObject),
		typeof(GUI),
		typeof(GUILayout),
		typeof(Input),
		typeof(LayerMask),
		typeof(Light),
		typeof(Material),
		typeof(Mathf),
		typeof(Mesh),
		typeof(MeshFilter),
		typeof(MeshRenderer),
		typeof(MonoBehaviour),
		typeof(UnityObject),
		typeof(PlayerPrefs),
		typeof(Physics),
		typeof(Physics2D),
		typeof(Quaternion),
		typeof(Ray),
		typeof(Rect),
		typeof(Rigidbody),
		typeof(Rigidbody2D),
		typeof(Screen),
		typeof(ScriptableObject),
		typeof(Time),
		typeof(Transform),
		typeof(Vector2),
		typeof(Vector3),
		typeof(Vector4),
		typeof(WWW),
	};

		public List<Type> current_types = new List<Type>();

		[SerializeField, HideInInspector]
		private SerializationData serializationData;

		public void OnAfterDeserialize() { 
			LoadData();
		}

		public void OnBeforeSerialize() {
			SaveData();
		}

		public void LoadData() {
			UnitySerializationUtility.DeserializeUnityObject(this, ref this.serializationData);
			if (current_types == null || current_types.Count == 0) {
				current_types = default_types;
			}
		}

		public void SaveData() {
			if (Application.isPlaying) return;
			if (current_types == null || current_types.Count == 0) {
				current_types = default_types;
			}
			UnitySerializationUtility.SerializeUnityObject(this, ref this.serializationData, true);
		}
	}
}
