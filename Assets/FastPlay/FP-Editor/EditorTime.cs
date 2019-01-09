#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace FastPlay.Editor {
	[InitializeOnLoad]
	public static class EditorTime {

		private static float last;

		private static float m_delta_time;

		private static float time_to_refresh;

		public static float fps { get; private set; }

		public static float time { get; private set; }

		public static float deltaTime {
			get {
				if (EditorApplication.isPlaying) {
					return Time.deltaTime;
				}
				return m_delta_time;
			}
			private set {
				m_delta_time = value;
			}
		}


		static EditorTime() {
			last = (float)EditorApplication.timeSinceStartup;
			EditorApplication.update += Update;
		}


		private static void Update() {
			deltaTime = (float)EditorApplication.timeSinceStartup - last;
			last = (float)EditorApplication.timeSinceStartup;
			time += deltaTime;
			time_to_refresh += deltaTime;
			if (time_to_refresh >= 1.0f) {
				RefreshFPS();
				time_to_refresh = 0.0f;
			}
		}

		private static void RefreshFPS() {
			fps = 1.0f / deltaTime;
		}
	}
}
#endif