using OdinSerializer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FastPlay.Runtime {
	public partial class GraphController : MonoBehaviour, ISerializationCallbackReceiver, ISupportsPrefabSerialization {

		SerializationData ISupportsPrefabSerialization.SerializationData { get { return this.serialization_data; } set { this.serialization_data = value; } }

		[SerializeField, HideInInspector]
		private SerializationData serialization_data;

		[SerializeField]
		private bool started;

		[SerializeField]
		private float time;

		public GraphAsset graph;

		public OnEnableAction on_enable = OnEnableAction.PlayGraph;

		public bool once = true;

		public float seconds;

		public OnDisableAction on_disable = OnDisableAction.StopGraph;

		public Dictionary<int, VariableObject> properties = new Dictionary<int, VariableObject>();

		public void OnAfterDeserialize() {
			LoadData();
		}

		public void OnBeforeSerialize() {
			SaveData();
		}

		public void LoadData() {
			UnitySerializationUtility.DeserializeUnityObject(this, ref this.serialization_data);
			//Debug.Log("LoadData");
		}

		public void SaveData() {
			if (Application.isPlaying) return;
			if (graph && graph.graph) {
				CopyParameters(graph.graph);
			}
			UnitySerializationUtility.SerializeUnityObject(this, ref this.serialization_data, true);
			//Debug.Log("SaveData");
		}

		public void OnGraphChange(GraphAsset last, GraphAsset current) {
			if (current == null || last == current) return;
			CopyParameters(current.graph);
		}

		public void CopyParameters() {
			if (graph && graph.graph) {
				CopyParameters(graph.graph);
			}
		}

		public void CopyParameters(Graph graph) {
			Dictionary<int, VariableObject> bkp = new Dictionary<int, VariableObject>(properties);
			properties.Clear();
			foreach (Parameter param in graph.variableParameters) {
				if (param) {
					VariableObject v;
					if (bkp.TryGetValue(param.id, out v)) {
						v.name = param.name;
						v.is_public = param.is_public;
						properties.Add(param.id, v);
					}
					else {
						AddVariable(param);
					}
				}
			}

			List<int> keys = new List<int>(properties.Keys.ToList());
			foreach (int key in keys) {
				if (!graph.variableParameters.Select(p => p.id).Contains(key)) {
					properties.Remove(key);
				}
			}
		}

		public VariableObject AddVariable(Parameter parameter) {
			VariableObject v;
			if (properties.TryGetValue(parameter.id, out v)) {
				v.name = parameter.name;
				v.is_public = parameter.is_public;
				return v;
			}
			VariableObject instance = (VariableObject)ObjectBase.CreateGenericInstance(typeof(VariableObject<>), parameter.valueType, parameter.id, parameter.name, parameter.is_public);
			properties[parameter.id] = instance;
			return instance;
		}

		public VariableObject AddVariable(int key, string name, Type type, bool is_public = true) {
			VariableObject v;
			if (properties.TryGetValue(key, out v)) {
				v.name = name;
				v.is_public = is_public;
				return v;
			}
			VariableObject instance = (VariableObject)ObjectBase.CreateGenericInstance(typeof(VariableObject<>), type, key, name, is_public);
			properties[key] = instance;
			return instance;
		}

		public VariableObject<T> AddVariable<T>(Parameter<T> parameter) {
			VariableObject v;
			if (properties.TryGetValue(parameter.id, out v)) {
				v.name = parameter.name;
				v.is_public = parameter.is_public;
				return (VariableObject<T>)v;
			}
			VariableObject<T> instance = ObjectBase.CreateInstance<VariableObject<T>>(parameter.id, parameter.name, parameter.is_public);
			properties[parameter.id] = instance;
			return instance;
		}

		public VariableObject<T> AddVariable<T>(int key, string name, bool is_public = true) {
			VariableObject v;
			if (properties.TryGetValue(key, out v)) {
				v.name = name;
				v.is_public = is_public;
				return (VariableObject<T>)v;
			}
			VariableObject<T> instance = ObjectBase.CreateInstance<VariableObject<T>>(key, name, is_public);
			properties[key] = instance;
			return instance;
		}

		public VariableObject<T> FindVariable<T>(string name) {
			return (VariableObject<T>)properties.Values.First(p => p.name == name);
		}

		public VariableObject GetVariable(int key) {
			return properties[key];
		}

		public VariableObject<T> GetVariable<T>(int key) {
			return (VariableObject<T>)properties[key];
		}

		public void OnControllerEnable() {
			CopyParameters();
			SetThisCurrent();
			if (!started) {
				switch (on_enable) {
					case OnEnableAction.DoNothing:
						break;
					case OnEnableAction.PlayGraph:
						this.PlayGraph();
						break;
					case OnEnableAction.WaitForSeconds:
						if (once) {
							if (seconds < (Time.time - time)) {
								this.PlayGraph();
							}
							else {
								StopCoroutine("PlayGraph");
								this.WaitForSecondsToCall(PlayGraph, (Time.time - time));
							}
						}
						else {
							StopCoroutine("PlayGraph");
							this.WaitForSecondsToCall(PlayGraph, seconds);
						}
						break;
				}
				started = true;
			}
		}

		public void OnControllerDisable() {
			switch (on_disable) {
				case OnDisableAction.DoNothing:
					break;
				case OnDisableAction.StopGraph:
					this.StopGraph();
					ClearEvents();
					break;
			}
			StopAllCoroutines2();
#if UNITY_EDITOR
			if (!UnityEditor.EditorApplication.isPlaying) {
				new log("yeah");
			}
#endif
		}

		public void OnControllerDestroy() {
			OnControllerDisable();
			if (graph) {
				graph.Finish();
				if (graph.isInstance) {
					DestroyImmediate(graph);
				}
			}
		}

		public void OnControllerStart() {
			SetThisCurrent();
			if (!started) {
				switch (on_enable) {
					case OnEnableAction.DoNothing:
						break;
					case OnEnableAction.PlayGraph:
						PlayGraph();
						break;
					case OnEnableAction.WaitForSeconds:
						if (once) {
							time = Time.time;
						}
						this.WaitForSecondsToCall(PlayGraph, seconds);
						break;
				}
				started = true;
			}
		}

		public void OnControllerUpdate() {
			SetThisCurrent();
			if (graph) {
				graph.Update();
			}
		}

		public void PlayGraph() {
			SetThisCurrent();
			if (graph) {
				if (!graph.isInstance) {
					graph = graph.GetClone();
				}
				graph.Play(this);
			}
		}

		public void StopGraph() {
			SetThisCurrent();
			if (graph) {
				graph.Stop();
				started = false;
			}
		}
	}
}
