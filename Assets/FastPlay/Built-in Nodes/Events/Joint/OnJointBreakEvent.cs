namespace FastPlay.Runtime {
	[BuiltInNode]
	[Icon("link_break")]
	[Title("OnJointBreak")]
	[Path("Events/Joint/OnJointBreak")]
	[Summary("Called when a joint attached to the same game object broke.")]
	public class OnJointBreakEvent : EventNode, IRegisterEvents, IRegisterPorts {

		public float breakForce;

		public void OnRegisterEvents() {
			Current.controller.DoJointBreak += OnJointBreak;
		}

		public void OnRemoveEvents() {
			Current.controller.DoJointBreak -= OnJointBreak;
		}

		public void OnRegisterPorts() {
			RegisterOutputValue<float>("breakForce", () => { return breakForce; });
		}

		public void OnJointBreak(float breakForce) {
			this.breakForce = breakForce;
			Call(output);
		}
	}
}
