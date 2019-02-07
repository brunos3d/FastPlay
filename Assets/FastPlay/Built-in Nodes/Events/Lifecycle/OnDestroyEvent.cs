namespace FastPlay.Runtime {
	[BuiltInNode]
	[Icon("cross")]
	[Title("OnDestroy")]
	[Path("Events/Lifecycle/OnDestroy")]
	[Summary("Destroying the attached Behaviour will result in the game or Scene receiving OnDestroy.")]
	public class OnDestroyEvent : EventNode {

		public void OnRegisterEvents() {
			Current.controller.DoDestroy += OnDestroy;
		}

		public void OnRemoveEvents() {
			Current.controller.DoDestroy -= OnDestroy;
		}

		public void OnDestroy() {
			Call(output);
		}
	}
}
