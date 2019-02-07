#if !(UNITY_2018_OR_NEWER || UNITY_2018 || UNITY_2018_2)
namespace FastPlay.Runtime {
	[BuiltInNode]
	[Icon("OnServerInitialized")]
	[Title("OnServerInitialized")]
	[Path("Events/Network/OnServerInitialized")]
	public class OnServerInitializedEvent : EventNode, IRegisterEvents {

		public void OnRegisterEvents() {
			Current.controller.DoServerInitialized += OnServerInitialized;
		}

		public void OnRemoveEvents() {
			Current.controller.DoServerInitialized -= OnServerInitialized;
		}

		public void OnServerInitialized() {
			Call(output);
		}
	}
}
#endif
