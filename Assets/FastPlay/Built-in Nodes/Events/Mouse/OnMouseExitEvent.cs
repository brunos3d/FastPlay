namespace FastPlay.Runtime {
	[BuiltInNode]
	[Icon("mouse_exit")]
	[Title("OnMouseExit")]
	[Path("Events/Mouse/OnMouseExit")]
	[Summary("Called when the mouse is not any longer over the GUIElement or Collider.")]
	public class OnMouseExitEvent : EventNode, IRegisterEvents {

		public void OnRegisterEvents() {
			Current.controller.DoMouseExit += OnMouseExit;
		}

		public void OnRemoveEvents() {
			Current.controller.DoMouseExit -= OnMouseExit;
		}

		public void OnMouseExit() {
			Call(output);
		}
	}
}
