using System;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Title("OnAudioFilterRead")]
	[Path("Events/Audio/OnAudioFilterRead")]
	public class OnAudioFilterReadEvent : EventNode, IRegisterEvents, IRegisterPorts {

		public float[] data;
		public int channels;

		public void OnRegisterEvents() {
			Current.controller.DoAudioFilterRead += OnAudioFilterRead;
		}

		public void OnRemoveEvents() {
			Current.controller.DoAudioFilterRead -= OnAudioFilterRead;
		}

		public void OnRegisterPorts() {
			RegisterOutputValue<float[]>("data", () => { return data; });
			RegisterOutputValue<int>("channels", () => { return channels; });
		}

		public void OnAudioFilterRead(float[] data, int channels) {
			this.data = data;
			this.channels = channels;
			Call(output);
		}
	}
}
