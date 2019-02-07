#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace FastPlay.Editor {
	public class AutoTypeInstance : ScriptableObject {

		private bool is_playing;

		private string current_text;

		private int chars_per_second;

		private float time_elapsed;

		private float time_to_change_elapsed;

		private float wait_to_change;

		private int current_tip;

		private bool random_tips;

		private string[] tips;

		public bool isPlaying {
			get {
				return is_playing;
			}
		}

		void OnEnable() {
			EditorApplication.update += Update;
		}

		void OnDisable() {
			EditorApplication.update -= Update;
		}

		public void Init(int chars_per_second, float wait_to_change, bool random_tips, params string[] tips) {
			this.chars_per_second = chars_per_second;
			this.wait_to_change = wait_to_change;
			this.random_tips = random_tips;
			this.tips = tips;
		}

		public void Play() {
			is_playing = true;
		}

		public void Stop() {
			is_playing = false;
		}

		public string GetCurrentText() {
			return current_text;
		}

		void Update() {
			try {
				if (is_playing) {
					if (tips[current_tip] == (current_text ?? string.Empty).Replace("|", string.Empty)) {
						time_to_change_elapsed += EditorTime.deltaTime;
						if (time_to_change_elapsed >= wait_to_change) {
							time_elapsed = 0.0f;
							time_to_change_elapsed = 0.0f;
							current_text = string.Empty;
							if (random_tips) {
								current_tip = UnityEngine.Random.Range(0, tips.Length - 1);
							}
							else {
								current_tip++;
								if (current_tip >= tips.Length) {
									current_tip = 0;
								}
							}
						}
					}
					else {
						time_elapsed += EditorTime.deltaTime;
						current_text = GetAutoTypeText(tips[current_tip], (int)(time_elapsed * chars_per_second));
					}
				}
				else {
					current_tip = 0;
					time_elapsed = 0.0f;
					time_to_change_elapsed = 0.0f;
					current_text = string.Empty;
				}
			}
			catch (Exception e) {
				DestroyImmediate(this);
				throw e;
			}
		}

		private string GetAutoTypeText(string text, int current_char) {
			if (current_char >= text.Length) {
				return text;
			}
			return text.Substring(0, current_char).Replace("|", string.Empty) + "|";
		}
	}
}
#endif
