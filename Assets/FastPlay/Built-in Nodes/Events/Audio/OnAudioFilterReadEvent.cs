namespace FastPlay.Runtime {
	[BuiltInNode]
	[Icon("AudioClip Icon")]
	[Title("OnAudioFilterRead")]
	[Path("Events/Audio/OnAudioFilterRead")]
	[Summary("OnAudioFilterRead is called every time a chunk of audio is sent to the filter (this happens frequently, every ~20ms depending on the sample rate and platform).")]
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
