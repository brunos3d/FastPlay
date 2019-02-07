namespace FastPlay.Runtime {
	[BuiltInNode]
	[Icon("application_pause")]
	[Title("OnApplicationPause")]
	[Path("Events/Application/OnApplicationPause")]
	[Summary("Sent to all GameObjects when the application pauses.")]
	public class OnApplicationPauseEvent : EventNode, IRegisterEvents, IRegisterPorts {

		public bool pause;

		public void OnRegisterEvents() {
			Current.controller.DoApplicationPause += OnApplicationPause;
		}

		public void OnRemoveEvents() {
			Current.controller.DoApplicationPause -= OnApplicationPause;
		}

		public void OnRegisterPorts() {
			RegisterOutputValue<bool>("pause", () => { return pause; });
		}

		public void OnApplicationPause(bool pause) {
			this.pause = pause;
			Call(output);
		}
	}
}
