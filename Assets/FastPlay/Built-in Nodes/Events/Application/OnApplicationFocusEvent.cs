namespace FastPlay.Runtime {
	[BuiltInNode]
	[Icon("application_focus")]
	[Title("OnApplicationFocus")]
	[Path("Events/Application/OnApplicationFocus")]
	[Summary("Sent to all GameObjects when the player gets or loses focus.")]
	public class OnApplicationFocusEvent : EventNode, IRegisterEvents, IRegisterPorts {

		public bool focus;

		public void OnRegisterEvents() {
			Current.controller.DoApplicationFocus += OnApplicationFocus;
		}

		public void OnRemoveEvents() {
			Current.controller.DoApplicationFocus -= OnApplicationFocus;
		}

		public void OnRegisterPorts() {
			RegisterOutputValue<bool>("focus", () => { return focus; });
		}

		public void OnApplicationFocus(bool focus) {
			this.focus = focus;
			Call(output);
		}
	}
}
