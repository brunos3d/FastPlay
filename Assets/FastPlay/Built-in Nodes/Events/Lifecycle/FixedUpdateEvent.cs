namespace FastPlay.Runtime {
	[BuiltInNode]
	[Icon("arrow_repeat")]
	[Title("FixedUpdate")]
	[Path("Events/Lifecycle/FixedUpdate")]
	[Summary("Frame-rate independent MonoBehaviour.FixedUpdate message for physics calculations.")]
	public class FixedUpdateEvent : EventNode, IRegisterEvents {

		public void OnRegisterEvents() {
			Current.controller.DoFixedUpdate += FixedUpdate;
		}

		public void OnRemoveEvents() {
			Current.controller.DoFixedUpdate -= FixedUpdate;
		}

		public void FixedUpdate() {
			Call(output);
		}
	}
}
