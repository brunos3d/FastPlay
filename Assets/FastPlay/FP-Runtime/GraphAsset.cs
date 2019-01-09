using OdinSerializer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FastPlay.Runtime {
	[CreateAssetMenu(fileName = "new GraphAsset", menuName = "GraphAsset", order = 81)]
	public class GraphAsset : ScriptableObject, ISerializationCallbackReceiver {

		[SerializeField]
		private SerializationData serialization_data;

		public string title;

		public string subtitle;

		public Vector2 position;

		[NonSerialized]
		public GraphController controller;

		[SerializeField, HideInInspector]
		private Graph m_graph;

		public bool destroying { get; private set; }

		public bool isInstance { get; private set; }

		public Graph graph {
			get {
				if (!m_graph) {
					m_graph = ObjectBase.CreateInstance<Graph>();
					m_graph.OnGraphAdd();
				}
				return m_graph;
			}
			private set {
				m_graph = value;
			}
		}

		public void OnEnable() {
			LoadData();
		}

		public void OnDestroy() {
			destroying = true;
		}

		public void OnAfterDeserialize() {
			LoadData();
		}

		public void OnBeforeSerialize() {
			SaveData();
		}

		public void Validate() {
			if (!m_graph) {
				m_graph = ObjectBase.CreateInstance<Graph>();
				m_graph.OnGraphAdd();
			}
			m_graph.Validate();
		}

		public void LoadData() {
			//m_graph = SerializationUtility.DeserializeValue<Graph>(Encoding.ASCII.GetBytes(m_data), DataFormat.JSON);
			UnitySerializationUtility.DeserializeUnityObject(this, ref this.serialization_data);
			if (!m_graph) {
				m_graph = ObjectBase.CreateInstance<Graph>();
				m_graph.OnGraphAdd();
			}
		}

		public void SaveData() {
			if (Application.isPlaying || isInstance) return;
			if (!m_graph) {
				m_graph = ObjectBase.CreateInstance<Graph>();
				m_graph.OnGraphAdd();
			}
			UnitySerializationUtility.SerializeUnityObject(this, ref this.serialization_data, true);
		}

		public List<Node> GetAllNodes() {
			return new List<Node>(graph.GetAllNodes()) { graph };
		}

		public void Play(GraphController controller) {
			if (!isInstance) return;
			this.controller = controller;
			graph.Play();
		}

		public void Stop() {
			if (!isInstance) return;
			graph.Stop();
		}

		public void Finish() {
			if (!isInstance) return;
			graph.Finish();
		}

		public void Update() {
			if (!isInstance) return;
			graph.Update();
		}

		public T AddNode<T>(Vector2 position = default(Vector2)) where T : Node {
			return graph.AddNode<T>(position);
		}

		public Node AddNode(Type type, Vector2 position = default(Vector2)) {
			return graph.AddNode(type, position);
		}

		public GraphAsset GetClone() {
			GraphAsset instance = Instantiate(this);
			instance.isInstance = true;
			return instance;
		}
	}
}
