namespace FastPlay.Runtime {
	[BuiltInNode]
	[Icon("mouse_over")]
	[Title("OnMouseOver")]
	[Path("Events/Mouse/OnMouseOver")]
	[Summary("Called every frame while the mouse is over the GUIElement or Collider.")]
	public class OnMouseOverEvent : EventNode, IRegisterEvents {

		public void OnRegisterEvents() {
			Current.controller.DoMouseOver += OnMouseOver;
		}

		public void OnRemoveEvents() {
			Current.controller.DoMouseOver -= OnMouseOver;
		}

		public void OnMouseOver() {
			Call(output);
		}
	}
}
