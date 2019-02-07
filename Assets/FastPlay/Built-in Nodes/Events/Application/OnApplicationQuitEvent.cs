namespace FastPlay.Runtime {
	[BuiltInNode]
	[Icon("application_quit")]
	[Title("OnApplicationQuit")]
	[Path("Events/Application/OnApplicationQuit")]
	[Summary("Sent to all game objects before the application quits.")]
	public class OnApplicationQuitEvent : EventNode, IRegisterEvents {

		public void OnRegisterEvents() {
			Current.controller.DoApplicationQuit += OnApplicationQuit;
		}

		public void OnRemoveEvents() {
			Current.controller.DoApplicationQuit -= OnApplicationQuit;
		}

		public void OnApplicationQuit() {
			Call(output);
		}
	}
}
